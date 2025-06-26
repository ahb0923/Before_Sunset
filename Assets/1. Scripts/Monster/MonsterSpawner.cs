using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPAWN_POINT_DIRECTION
{
    Up,
    Right,
    Down,
    Left,
}

public class MonsterSpawner : MonoBehaviour
{
    [Header("# Map Setting")]
    [SerializeField] private Vector2 _mapSize;
    private Vector3 _bottomLeftPosition;
    [SerializeField] private float _nodeSize;
    private float _nodeHalfSize => _nodeSize * 0.5f;
    private Node[,] _nodeGrid;
    private int _nodeCountX;
    private int _nodeCountY;
    [SerializeField] private LayerMask _obstacleMask;

    private Dictionary<SPAWN_POINT_DIRECTION, Dictionary<int, List<Node>>> _pathCache = new();

    [Header("# Spawn Setting")]
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnTime;
    [SerializeField] private List<Monster_SO> _monsterDatas;

    [Header("# Test")]
    public Transform TestCore; // 인스펙터에서 넣어도 되고 안 넣어도 됨
    public int testStage;
    
    private void Awake()
    {
        if(TestCore == null)
            TestCore = GameObject.Find("TestCore")?.GetComponent<Transform>();

        if(TestCore == null)
        {
            Debug.LogError("[MonsterSpawner] TestCore를 찾지 못했습니다.");
        }

        GenerateNodes();
    }

    /// <summary>
    /// 맵 사이즈와 노드 사이즈를 바탕으로 노드를 생성
    /// </summary>
    private void GenerateNodes()
    {
        _nodeCountX = Mathf.CeilToInt(_mapSize.x / _nodeSize);
        _nodeCountY = Mathf.CeilToInt(_mapSize.y / _nodeSize);
        _nodeGrid = new Node[_nodeCountX, _nodeCountY];

        _bottomLeftPosition = new Vector2(transform.position.x, transform.position.y) - _mapSize * 0.5f;
        for (int x = 0; x < _nodeCountX; x++)
        {
            for (int y = 0; y < _nodeCountY; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = _bottomLeftPosition + new Vector3(x * _nodeSize + _nodeHalfSize, y * _nodeSize + _nodeHalfSize);
                Collider2D hit = Physics2D.OverlapBox(worldPos, new Vector2(_nodeHalfSize, _nodeHalfSize), 0, _obstacleMask);
                _nodeGrid[x, y] = new Node(hit == null, gridPos, worldPos);
            }
        }
    }

    /// <summary>
    /// 월드 포지션에 대한 가장 가까운 노드를 반환
    /// </summary>
    private Node GetNearestNode(Vector3 worldPos)
    {
        Vector2 relativePos = worldPos - _bottomLeftPosition;
        Vector2Int gridIndex = Vector2Int.RoundToInt(relativePos / _nodeSize);

        // 유효 범위로 클램핑
        gridIndex.x = Mathf.Clamp(gridIndex.x, 0, _nodeCountX - 1);
        gridIndex.y = Mathf.Clamp(gridIndex.y, 0, _nodeCountY - 1);

        return _nodeGrid[gridIndex.x, gridIndex.y];
    }

    /// <summary>
    /// Obstacle 레이어 콜라이더를 기반으로 노드 Walkable 업데이트
    /// </summary>
    [ContextMenu("맵 업데이트")]
    public void UpdateNodesWalkable()
    {
        foreach (Node node in _nodeGrid)
        {
            Collider2D hit = Physics2D.OverlapBox(node.WorldPos, new Vector2(_nodeHalfSize, _nodeHalfSize), 0, _obstacleMask);
            node.isWalkable = hit == null;
        }

        foreach(var dict in _pathCache.Values)
        {
            foreach(var key in dict.Keys)
            {
                bool isChanged = false;

                List<Node> path = dict[key];
                foreach(Node node in path)
                {
                    if (!node.isWalkable)
                    {
                        isChanged = true;
                        break;
                    }
                }

                if (isChanged)
                {
                    dict.Remove(key);
                }
            }
        }
    }

    /// <summary>
    /// 캐싱된 길이 있는 지 확인하고 없으면 생성하여 반환
    /// </summary>
    public List<Node> GetCachedPath(SPAWN_POINT_DIRECTION dir, int monsterSize)
    {
        Node startNode = GetNearestNode(_spawnPoints[(int)dir].position);
        Node endNode = GetNearestNode(TestCore.position);

        // 방향별 딕셔너리 확보
        if (!_pathCache.TryGetValue(dir, out var sizeDict))
        {
            sizeDict = new Dictionary<int, List<Node>>();
            _pathCache[dir] = sizeDict;
        }

        // 사이즈별 경로 확인
        if (sizeDict.TryGetValue(monsterSize, out var cachedPath))
        {
            return cachedPath;
        }

        // 경로가 없으면 계산 후 저장
        List<Node> path = AstarAlgorithm.FindPath(_nodeGrid, startNode, endNode, monsterSize);
        if (path != null)
        {
            sizeDict[monsterSize] = path;
        }

        return path;
    }

    /// <summary>
    /// 스테이지에 따른 모든 몬스터 소환
    /// </summary>
    /// <param name="stage"></param>
    [ContextMenu("몬스터 소환")]
    public void SpawnAllMonsters()
    {
        StartCoroutine(C_SpawnMonsters());
    }

    private IEnumerator C_SpawnMonsters()
    {
        int count = 0;
        float timer = 0f;

        while (true)
        {
            if(count >= testStage * 20) // test용
                yield break;

            timer += Time.deltaTime;
            if(timer >= _spawnTime)
            {
                timer = 0f;
                count++;

                // 지금은 일단 스테이지마다 무슨 몬스터를 몇 마리 소환할 지 모르니까 Walker만 소환
                SpawnMonster();
            }

            yield return null;
        }
    }

    private void SpawnMonster()
    {
        int randInt = Random.Range(0, _spawnPoints.Length);
        Vector3 pos = _spawnPoints[randInt].position;
        List<Node> path = GetCachedPath((SPAWN_POINT_DIRECTION)randInt, 1);

        GameObject obj = PoolManager.Instance.GetFromPool(POOL_TYPE.Monster);
        obj.GetComponent<MonsterHandler>().Init(_monsterDatas[0], pos, TestCore, path);
    }

    #region Editor Only
    private void OnDrawGizmos()
    {
        // 맵 사이즈 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _mapSize);

        // 노드 시각화
        if (_nodeGrid != null)
        {
            foreach (Node node in _nodeGrid)
            {
                Gizmos.color = node.isWalkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(node.WorldPos, new Vector2(_nodeHalfSize, _nodeHalfSize));
            }
        }
    }
    #endregion
}
