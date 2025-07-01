using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoSingleton<MapManager>
{
    [Header("# Map Setting")]
    [SerializeField] private Vector3 _mapCenter = Vector3.zero;
    [SerializeField] private Vector2 _mapSize;
    [SerializeField] private int _nodeSize = 1;
    private NodeGrid _grid;

    public Core Core { get; private set; }
    public Tilemap GroundTile { get; private set; }
    public BuildPreview BuildPreview { get; private set; }
    public DragIcon DragIcon { get; private set; }

    // 맵 장애물 설정
    private int _nextId;
    private Stack<int> _walkableIdStack = new Stack<int>();
    private Dictionary<Transform, int> _walkableIdDict = new Dictionary<Transform, int>();
    private Dictionary<Transform, int> _obstacleSizeDict = new Dictionary<Transform, int>();

    protected override void Awake()
    {
        base.Awake();

        Core = GetComponentInChildren<Core>();
        GroundTile = GetComponentInChildren<Tilemap>();
        BuildPreview = GetComponentInChildren<BuildPreview>();
        DragIcon = GetComponentInChildren<DragIcon>(true);

        InitGridAndWalkableStack();
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
    }

    /// <summary>
    /// 코어, 타워 등의 장애물 transform을 딕셔너리에서 제거
    /// </summary>
    /// <param name="size">장애물 사이즈 (1x1이면 1)</param>
    public void RemoveObstacle(Transform obstacle, int size)
    {
        int walkableId = _walkableIdDict[obstacle];
        if (!_walkableIdStack.Contains(walkableId)) _walkableIdStack.Push(walkableId);

        _walkableIdDict.Remove(obstacle);
        _obstacleSizeDict.Remove(obstacle);
        _grid.SetWalkableIndex(-1, obstacle.position, size);
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
    /// 맵 사이즈 & 노드 상태 확인
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, _mapSize);

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
}
