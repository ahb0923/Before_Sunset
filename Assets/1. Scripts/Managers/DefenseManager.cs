using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct ObstacleData
{
    public int walkableId;
    public int obstacleSize;

    public ObstacleData(int walkableId, int obstacleSize)
    {
        this.walkableId = walkableId;
        this.obstacleSize = obstacleSize;
    }
}

public class DefenseManager : MonoSingleton<DefenseManager>, ISaveable
{
    [Header("# Map Setting")]
    [SerializeField] private Vector3 _mapCenter;
    [SerializeField] private Vector2 _mapSize;
    [SerializeField] private int _nodeSize = 1;
    private NodeGrid _grid;

    [Header("# A Star Setting")]
    [SerializeField] private int _monsterPenalty = 5;

    // 맵 장애물(코어 & 타워) 설정
    private int _nextId;
    private Stack<int> _walkableIdStack = new Stack<int>();
    private Dictionary<Transform, ObstacleData> _obstacleDict = new Dictionary<Transform, ObstacleData>();
    private Dictionary<int, List<Transform>> _distFromCoreDict = new Dictionary<int, List<Transform>>();

    public Core Core { get; private set; }
    public MonsterSpawner MonsterSpawner { get; private set; }
    public Tilemap GroundTile { get; private set; }
    public BuildPreview BuildPreview { get; private set; }
    public DragIcon DragIcon { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Core = GetComponentInChildren<Core>();
        if (MonsterSpawner == null)
        {
            MonsterSpawner = FindObjectOfType<MonsterSpawner>();
        }

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
        AstarAlgorithm.BindMonsterPenalty(_monsterPenalty);

        _nextId = 0;
        AddObstacle(Core.transform, Core.Size);
    }

    /// <summary>
    /// 코어, 타워 등의 장애물 transform을 딕셔너리에 추가
    /// </summary>
    /// <param name="size">장애물 사이즈 (1x1이면 1)</param>
    public void AddObstacle(Transform obstacle, int size)
    {
        // walkabla ID 생성
        int walkableId;
        if (_walkableIdStack.Count > 0) walkableId = _walkableIdStack.Pop();
        else walkableId = _nextId++;

        // 노드 그리드에 사이즈에 따른 walkable ID 부여
        ObstacleData data = new ObstacleData(walkableId, size);
        _obstacleDict[obstacle] = data;
        _grid.SetWalkableIndex(obstacle.position, data);

        // 코어와 떨어진 거리 관련 타워 리스트에 추가
        int dist = GetChebyshevDistanceFromCore(obstacle.position);
        if (!_distFromCoreDict.ContainsKey(dist)) _distFromCoreDict[dist] = new List<Transform>();
        _distFromCoreDict[dist].Add(obstacle);

        // 퀘스트 매니저에 건물 설치 알림
        if(obstacle.TryGetComponent<IPoolable>(out var poolable))
        {
            QuestManager.Instance?.AddQuestAmount(QUEST_TYPE.PlaceBuilding, poolable.GetId());
        }

        // 몬스터 경로 재탐색 이벤트 호출
        MonsterSpawner.OnObstacleChanged();
    }

    /// <summary>
    /// 코어, 타워 등의 장애물 transform을 딕셔너리에서 제거
    /// </summary>
    public void RemoveObstacle(Transform obstacle)
    {
        // walkabla ID 반환
        int walkableId = _obstacleDict[obstacle].walkableId;
        if (!_walkableIdStack.Contains(walkableId)) _walkableIdStack.Push(walkableId);

        // 노드 그리드에서 해당 타워의 walkable ID 제거
        _grid.SetWalkableIndex(obstacle.position, _obstacleDict[obstacle].obstacleSize);

        // 딕셔너리에 해당 타워 정보 제거
        _obstacleDict.Remove(obstacle);
        _distFromCoreDict[GetChebyshevDistanceFromCore(obstacle.position)].Remove(obstacle);

        // 몬스터 경로 재탐색 이벤트 호출
        MonsterSpawner.OnObstacleChanged();
    }

    /// <summary>
    /// 코어에서 dist만큼 떨어진 타겟 리스트를 반환
    /// </summary>
    public List<Transform> GetTargetList(int dist)
    {
        int defaultDist = Core.Size / 2 + 2;

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
        return AstarAlgorithm.FindPathToTarget(startNode, size, targetNode, _obstacleDict[target].walkableId, _obstacleDict[target].obstacleSize);
    }

    /// <summary>
    /// 장애물의 Walkable Id를 반환<br/>
    /// 만약, 딕셔너리에 없다면 -1을 반환
    /// </summary>
    /// <param name="obstacle"></param>
    public int GetWalkableId(Transform obstacle)
    {
        if (!_obstacleDict.ContainsKey(obstacle)) return -1;

        return _obstacleDict[obstacle].walkableId;
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
    }

    /// <summary>
    /// 모든 빌딩 데이터 정보 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        foreach (Transform constructed in _obstacleDict.Keys)
        {
            if (constructed.TryGetComponent<TowerStatHandler>(out var tower))
            {
                // 일단, 업그레이드 관련 정보가 없어서 임시로 Normal 사용
                TowerSaveData towerData = new TowerSaveData(tower.ID, tower.transform.position, BUILDING_TYPE.Normal, (int)tower.CurrHp);
                data.constructedTowers.Add(towerData);
            }
            else if(constructed.TryGetComponent<Smelter>(out var smelter))
            {
                ItemSaveData inputItemData = null;
                if(smelter.InputItem != null)
                {
                    inputItemData = new ItemSaveData(smelter.InputItem.Data.id, smelter.InputItem.stack);
                }

                ItemSaveData outputItemData = null;
                if(smelter.OutputItem != null)
                {
                    outputItemData = new ItemSaveData(smelter.OutputItem.Data.id, smelter.OutputItem.stack);
                }

                SmelterSaveData smelterData = new SmelterSaveData(smelter.smelterID, smelter.transform.position, inputItemData, smelter.elapsed, outputItemData);
                data.constructedSmelters.Add(smelterData);
            }
        }
    }

    /// <summary>
    /// 모든 빌딩 데이터 정보 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        // 기존 빌딩들 풀에 반환
        foreach(Transform towerObj in _obstacleDict.Keys)
        {
            if (towerObj.TryGetComponent<IPoolable>(out var poolable))
            {
                PoolManager.Instance.ReturnToPool(poolable.GetId(), towerObj.gameObject);
            }
        }
        
        // 컬렉션 데이터 초기화
        _walkableIdStack.Clear();
        _obstacleDict.Clear();
        _distFromCoreDict.Clear();

        // 코어 옵스터클에 추가
        _nextId = 0;
        AddObstacle(Core.transform, Core.Size);

        // 로드된 타워 설치
        foreach (TowerSaveData towerData in data.constructedTowers)
        {
            GameObject obj = PoolManager.Instance.GetFromPool(towerData.towerId, towerData.position);
            AddObstacle(obj.transform, 1); // 타워 & 제련소 사이즈 현재는 1

            if (obj.TryGetComponent<TowerStatHandler>(out var tower))
            {
                tower.CurrHp = towerData.curHp;
                // 일단, 업그레이드 관련 정보가 없어서 로드는 안하고 있음
            }
        }

        // 로드된 제련소 설치
        foreach (SmelterSaveData smelterData in data.constructedSmelters)
        {
            GameObject obj = PoolManager.Instance.GetFromPool(smelterData.smelterId, smelterData.position);
            AddObstacle(obj.transform, 1); // 타워 & 제련소 사이즈 현재는 1

            if (obj.TryGetComponent<Smelter>(out var smelter))
            {
                Item inputItem = null;
                if(smelterData.inputItem.itemId != 0)
                {
                    inputItem = new Item(DataManager.Instance.ItemData.GetById(smelterData.inputItem.itemId));
                    inputItem.stack = smelterData.inputItem.quantity;
                }

                Item outputItem = null;
                if (smelterData.outputItem.itemId != 0)
                {
                    outputItem = new Item(DataManager.Instance.ItemData.GetById(smelterData.outputItem.itemId));
                    outputItem.stack = smelterData.outputItem.quantity;
                }

                smelter.SetOutputItem(outputItem);
                smelter.SetInputItem(inputItem, smelterData.smeltedTime);
            }
        }
    }
}
