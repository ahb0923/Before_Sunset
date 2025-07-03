using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoSingleton<MapManager>
{
    [Header("# Map Setting")]
    [SerializeField] private Vector3 _mapCenter;
    [SerializeField] private Vector2 _mapSize;
    [SerializeField] private int _nodeSize = 1;
    private NodeGrid _grid;

    // 맵 장애물(코어 & 타워) 설정
    private int _nextId;
    private Stack<int> _walkableIdStack = new Stack<int>();
    private Dictionary<Transform, int> _walkableIdDict = new Dictionary<Transform, int>();
    private Dictionary<Transform, int> _obstacleSizeDict = new Dictionary<Transform, int>();
    private Dictionary<int, List<Transform>> _distFromCoreDict = new Dictionary<int, List<Transform>>();

    public Core Core { get; private set; }
    [field: SerializeField] public MonsterSpawner MonsterSpawner { get; private set; }
    // ========== [맵 관련 필드] ==========
    [Header("맵 관련 설정")]
    [SerializeField] private GameObject[] mapChunks;
    [SerializeField] private Transform player;

    private int currentMapIndex = 0;
    private int previousMapIndex = -1;

    // ========== [맵 구성 요소] ==========
    public Tilemap GroundTile { get; private set; }
    public BuildPreview BuildPreview { get; private set; }
    public DragIcon DragIcon { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Core = GetComponentInChildren<Core>();
        if(MonsterSpawner == null)
        {
            MonsterSpawner = FindObjectOfType<MonsterSpawner>();
        }

        GroundTile = GetComponentInChildren<Tilemap>();
        BuildPreview = GetComponentInChildren<BuildPreview>();
        DragIcon = GetComponentInChildren<DragIcon>(true);

        InitGridAndWalkableStack();

        // 모든 맵 중 기본 맵(0번)만 활성화 (LKM)
        for (int i = 0; i < mapChunks.Length; i++)
        {
            mapChunks[i].SetActive(i == 0);
        }

        currentMapIndex = 0;
        previousMapIndex = -1;
    }

    /// <summary>
    /// 노드 그리드 생성 & 코어 위치 설정
    /// </summary>
    private void InitGridAndWalkableStack()
    {
        _grid = new NodeGrid(_mapCenter, _mapSize, _nodeSize);
        AstarAlgorithm.BindGrid(_grid);

        _nextId = 0;
        AddObstacle(Core.transform, Core.Size);
    }

    /// <summary>
    /// 코어, 타워 등의 장애물 transform을 딕셔너리에 추가
    /// </summary>
    /// <param name="size">장애물 사이즈 (1x1이면 1)</param>
    public void AddObstacle(Transform obstacle, int size)
    {
        int walkableId;
        if(_walkableIdStack.Count > 0) walkableId = _walkableIdStack.Pop();
        else walkableId = _nextId++;

        _walkableIdDict[obstacle] = walkableId;
        _obstacleSizeDict[obstacle] = size;
        _grid.SetWalkableIndex(walkableId, obstacle.position, size);

        int dist = GetChebyshevDistanceFromCore(obstacle.position);
        if (!_distFromCoreDict.ContainsKey(dist)) _distFromCoreDict[dist] = new List<Transform>();
        _distFromCoreDict[dist].Add(obstacle);
    }

    /// <summary>
    /// 코어, 타워 등의 장애물 transform을 딕셔너리에서 제거
    /// </summary>
    public void RemoveObstacle(Transform obstacle)
    {
        int walkableId = _walkableIdDict[obstacle];
        if (!_walkableIdStack.Contains(walkableId)) _walkableIdStack.Push(walkableId);

        _grid.SetWalkableIndex(-1, obstacle.position, _obstacleSizeDict[obstacle]);
        _walkableIdDict.Remove(obstacle);
        _obstacleSizeDict.Remove(obstacle);

        _distFromCoreDict[GetChebyshevDistanceFromCore(obstacle.position)].Remove(obstacle);

        MonsterSpawner.OnObstacleDestroyed();
    }

    /// <summary>
    /// 코어에서 dist만큼 떨어진 타겟 리스트를 반환
    /// </summary>
    public List<Transform> GetTargetList(int dist)
    {
        int defaultDist = Core.Size / 2 + 1;

        if (_distFromCoreDict.ContainsKey(defaultDist + dist))
            return _distFromCoreDict[defaultDist + dist];
        else
            return null;
    }

    /// <summary>
    /// 시작 위치에서 타겟을 향한 경로를 반환
    /// </summary>
    /// <param name="startPos">시작 위치</param>
    /// <param name="size">오브젝트 사이즈 (1x1이면 1)</param>
    /// <param name="target">타겟 트랜스폼</param>
    /// <returns></returns>
    public NodePath FindPathToTarget(Vector3 startPos, int size, Transform target)
    {
        Node startNode = _grid.GetNode(startPos);
        Node targetNode = _grid.GetNode(target.position);
        return AstarAlgorithm.FindPathToTarget_Bind(startNode, size, targetNode, _walkableIdDict[target], _obstacleSizeDict[target]);
    }

    /// <summary>
    /// 장애물의 Walkable Id를 반환<br/>
    /// 만약, 딕셔너리에 없다면 -1을 반환
    /// </summary>
    /// <param name="obstacle"></param>
    public int GetWalkableId(Transform obstacle)
    {
        if(!_walkableIdDict.ContainsKey(obstacle)) return -1;

        return _walkableIdDict[obstacle];
    }

    /// <summary>
    /// 코어로부터 체비쇼프 거리를 반환<br/>
    /// ※ 체비쇼프 거리는 상하좌우나 대각선에서 1타일 떨어져 있으면 모두 1을 반환
    /// </summary>
    private int GetChebyshevDistanceFromCore(Vector3 targetPos)
    {
        Vector2Int coreGridIndex = _grid.GetGridIndex(Core.transform.position);
        Vector2Int targetGridIndex = _grid.GetGridIndex(targetPos);

        return Mathf.Max(Mathf.Abs(coreGridIndex.x - targetGridIndex.x), Mathf.Abs(coreGridIndex.y - targetGridIndex.y));
    }

    /// <summary>
    /// 맵 사이즈 & 노드 상태 확인
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_mapCenter, _mapSize);

        if(_grid != null)
        {
            Vector3 nodeQuarterSize = new Vector3(_nodeSize * 0.25f, _nodeSize * 0.25f);
            foreach(Node node in _grid.Nodes)
            {
                switch (node.walkableIndex)
                {
                    case -1:
                        Gizmos.color = Color.green;
                        break;

                    case 0:
                        Gizmos.color = Color.black;
                        break;

                    default:
                        Gizmos.color = Color.red;
                        break;
                }
                Gizmos.DrawCube(node.WorldPos, nodeQuarterSize);
            }
        }
    }
        

    // ========== [맵 전환 메서드] ==========(LKM)
    public void MoveToRandomMap()
    {
        int nextIndex;

        if (mapChunks.Length <= 1) return;

        do
        {
            nextIndex = Random.Range(1, mapChunks.Length);
        } while (nextIndex == currentMapIndex);

        MoveToMap(nextIndex);
    }

    public void MoveToPreviousMap()
    {
        if (previousMapIndex == -1) return;

        MoveToMap(previousMapIndex);
    }

    private void MoveToMap(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= mapChunks.Length) return;

        mapChunks[currentMapIndex].SetActive(false);

        mapChunks[targetIndex].SetActive(true);

        previousMapIndex = currentMapIndex;
        currentMapIndex = targetIndex;

        player.position = mapChunks[targetIndex].transform.position + new Vector3(0f, 0f, 0f);
    }
}
