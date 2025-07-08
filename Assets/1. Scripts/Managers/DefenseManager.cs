using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DefenseManager : MonoSingleton<DefenseManager>
{
    [Header("# Map Setting")]
    [SerializeField] private Vector3 _mapCenter;
    [SerializeField] private Vector2 _mapSize;
    [SerializeField] private int _nodeSize = 1;
    private NodeGrid _grid;

    [Header("# A Star Setting")]
    [SerializeField] private int _monsterPenalty = 5;

    // �� ��ֹ�(�ھ� & Ÿ��) ����
    private int _nextId;
    private Stack<int> _walkableIdStack = new Stack<int>();
    private Dictionary<Transform, int> _walkableIdDict = new Dictionary<Transform, int>();
    private Dictionary<Transform, int> _obstacleSizeDict = new Dictionary<Transform, int>();
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
    /// ��� �׸��� ���� & �ھ� ��ġ ����
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
    /// �ھ�, Ÿ�� ���� ��ֹ� transform�� ��ųʸ��� �߰�
    /// </summary>
    /// <param name="size">��ֹ� ������ (1x1�̸� 1)</param>
    public void AddObstacle(Transform obstacle, int size)
    {
        int walkableId;
        if (_walkableIdStack.Count > 0) walkableId = _walkableIdStack.Pop();
        else walkableId = _nextId++;

        _walkableIdDict[obstacle] = walkableId;
        _obstacleSizeDict[obstacle] = size;
        _grid.SetWalkableIndex(walkableId, obstacle.position, size);

        int dist = GetChebyshevDistanceFromCore(obstacle.position);
        if (!_distFromCoreDict.ContainsKey(dist)) _distFromCoreDict[dist] = new List<Transform>();
        _distFromCoreDict[dist].Add(obstacle);

        MonsterSpawner.OnObstacleChanged();
    }

    /// <summary>
    /// �ھ�, Ÿ�� ���� ��ֹ� transform�� ��ųʸ����� ����
    /// </summary>
    public void RemoveObstacle(Transform obstacle)
    {
        int walkableId = _walkableIdDict[obstacle];
        if (!_walkableIdStack.Contains(walkableId)) _walkableIdStack.Push(walkableId);

        _grid.SetWalkableIndex(-1, obstacle.position, _obstacleSizeDict[obstacle]);
        _walkableIdDict.Remove(obstacle);
        _obstacleSizeDict.Remove(obstacle);

        _distFromCoreDict[GetChebyshevDistanceFromCore(obstacle.position)].Remove(obstacle);

        MonsterSpawner.OnObstacleChanged();
    }

    /// <summary>
    /// �ھ�� dist��ŭ ������ Ÿ�� ����Ʈ�� ��ȯ
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
    /// ���� ��ġ���� Ÿ���� ���� ��θ� ��ȯ
    /// </summary>
    /// <param name="startPos">���� ��ġ</param>
    /// <param name="size">������Ʈ ������ (1x1�̸� 1)</param>
    /// <param name="target">Ÿ�� Ʈ������</param>
    /// <returns></returns>
    public NodePath FindPathToTarget(Vector3 startPos, int size, Transform target)
    {
        Node startNode = _grid.GetNode(startPos);
        Node targetNode = _grid.GetNode(target.position);
        return AstarAlgorithm.FindPathToTarget(startNode, size, targetNode, _walkableIdDict[target], _obstacleSizeDict[target]);
    }

    /// <summary>
    /// ��ֹ��� Walkable Id�� ��ȯ<br/>
    /// ����, ��ųʸ��� ���ٸ� -1�� ��ȯ
    /// </summary>
    /// <param name="obstacle"></param>
    public int GetWalkableId(Transform obstacle)
    {
        if (!_walkableIdDict.ContainsKey(obstacle)) return -1;

        return _walkableIdDict[obstacle];
    }

    /// <summary>
    /// �ھ�κ��� ü����� �Ÿ��� ��ȯ<br/>
    /// �� ü����� �Ÿ��� �����¿쳪 �밢������ 1Ÿ�� ������ ������ ��� 1�� ��ȯ
    /// </summary>
    private int GetChebyshevDistanceFromCore(Vector3 targetPos)
    {
        Vector2Int coreGridIndex = _grid.GetGridIndex(Core.transform.position);
        Vector2Int targetGridIndex = _grid.GetGridIndex(targetPos);

        return Mathf.Max(Mathf.Abs(coreGridIndex.x - targetGridIndex.x), Mathf.Abs(coreGridIndex.y - targetGridIndex.y));
    }

    /// <summary>
    /// �� ������ & ��� ���� Ȯ��
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_mapCenter, _mapSize);
    }
}
