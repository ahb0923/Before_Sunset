using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [SerializeField] private Vector2 _mapSize;
    private Vector3 _bottomLeftPosition;
    [SerializeField] private float _nodeSize = 1f;
    private Node[,] _nodeGrid;
    private int _nodeCountX;
    private int _nodeCountY;

    [SerializeField] private LayerMask _obstacleMask;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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

        _bottomLeftPosition = new Vector2(transform.position.x, transform.position.y) - _mapSize / 2f;
        for (int x = 0; x < _nodeCountX; x++)
        {
            for (int y = 0; y < _nodeCountY; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 pos = GetWorldPosition(gridPos);
                Collider2D hit = Physics2D.OverlapBox(pos, new Vector2(_nodeSize / 2f, _nodeSize / 2f), 0, _obstacleMask);
                _nodeGrid[x, y] = new Node(hit == null, gridPos);
            }
        }
    }

    /// <summary>
    /// 해당 노드에 대한 월드 포지션을 반환
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(Node node)
    {
        return _bottomLeftPosition + new Vector3((node.gridIndex.x + 0.5f) * _nodeSize, (node.gridIndex.y + 0.5f) * _nodeSize);
    }

    /// <summary>
    /// 해당 그리드 인덱스(x, y)에 대한 월드 포지션을 반환
    /// </summary>
    /// <param name="gridIndex"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(Vector2Int gridIndex)
    {
        return _bottomLeftPosition + new Vector3((gridIndex.x + 0.5f) * _nodeSize, (gridIndex.y + 0.5f) * _nodeSize);
    }

    /// <summary>
    /// 월드 포지션에 대한 노드를 반환
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    private Node GetNode(Vector3 worldPos)
    {
        Vector3 pos = worldPos - _bottomLeftPosition;
        Vector2Int gridIndex = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        if (gridIndex.x < 0 || gridIndex.y < 0 || gridIndex.x >= _nodeCountX || gridIndex.y >= _nodeCountY)
            return null;

        return _nodeGrid[gridIndex.x, gridIndex.y];
    }

    /// <summary>
    /// 시작 지점과 목표 지점에 따른 가장 빠른 path 노드 리스트 반환
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public List<Node> FindPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = GetNode(startPos);
        if (startNode == null)
        {
            Debug.LogWarning("시작 지점 근처에 노드가 없습니다.");
            return null;
        }

        Node endNode = GetNode(endPos);
        if (startNode == null)
        {
            Debug.LogWarning("목표 지점 근처에 노드가 없습니다.");
            return null;
        }

        // 모든 노드 G 코스트 초기화
        foreach (Node node in _nodeGrid)
        {
            node.GCost = int.MaxValue;
        }

        // 시작 노드 & 오픈 리스트 초기화
        List<Node> openList = new List<Node>();
        startNode.GCost = 0;
        startNode.HCost = GetPathfindingDistance(startNode, endNode);
        openList.Add(startNode);

        int lowestF;
        Node curNode;
        while (openList.Count > 0)
        {
            // F 비용이 가장 낮은 노드 → H 비용이 가장 낮은 노드 순으로 탐색
            lowestF = 0;
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost > openList[lowestF].FCost) continue;
                if ((openList[i].FCost == openList[lowestF].FCost) && (openList[i].HCost >= openList[lowestF].HCost)) continue;

                lowestF = i;
            }
            curNode = openList[lowestF];

            // F 비용이 가장 낮은 노드는 탐색할거니 오픈 리스트에서 제거
            openList.RemoveAt(lowestF);
            
            // 목표 노드에 도달했을 경우, 길을 역추적하여 반환
            if(curNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach(Node connectedNode in GetNeighborNodes(curNode))
            {
                int updatedGCost = curNode.GCost + GetPathfindingDistance(curNode, connectedNode);

                // 검색해야 할 이웃 노드 오픈 리스트에 추가 & 코스트와 부모 노드 업데이트
                if (updatedGCost < connectedNode.GCost)
                {
                    connectedNode.parent = curNode;
                    connectedNode.GCost = updatedGCost;
                    connectedNode.HCost = GetPathfindingDistance(connectedNode, endNode);

                    if (!openList.Contains(connectedNode))
                    {
                        openList.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 길찾기 로직에 따른 거리 계산
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int GetPathfindingDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.gridIndex.x - b.gridIndex.x);
        int distY = Mathf.Abs(a.gridIndex.y - b.gridIndex.y);

        return 14 * Mathf.Min(distX, distY) + 10 * Mathf.Abs(distX - distY);
    }

    /// <summary>
    /// 부모 노드를 역추적해서 path 노드 리스트 반환
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns></returns>
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Add(startNode);

        path.Reverse();

        return path;
    }

    /// <summary>
    /// 이웃 노드에 대한 리스트를 반환
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighborList = new List<Node>();
        for (int x = node.gridIndex.x - 1; x <= node.gridIndex.x + 1; x++)
        {
            for (int y = node.gridIndex.y - 1; y <= node.gridIndex.y + 1; y++) 
            {
                if (x < 0 || y < 0 || x >= _nodeCountX || y >= _nodeCountY) continue; // 범위 밖 제외
                if (!node.isWalkable) continue; // 해당 노드로 갈 수 없으면 제외
                if (x == node.gridIndex.x && y == node.gridIndex.y) continue; // 자기 자신 제외
                if ((Mathf.Abs(x - node.gridIndex.x) == 1) && (Mathf.Abs(y - node.gridIndex.y) == 1)) continue; // 대각선 제외

                neighborList.Add(_nodeGrid[x, y]);
            }
        }

        return neighborList;
    }

    #region Editor Only
    private void OnValidate()
    {
        // _nodeSize를 0.5 단위로 변경
        _nodeSize = Mathf.Round(_nodeSize * 2f) / 2f;
    }

    private void OnDrawGizmos()
    {
        // 맵 사이즈 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _mapSize);

        // 노드 시각화
        if(_nodeGrid != null)
        {
            foreach(Node node in _nodeGrid)
            {
                Gizmos.color = node.isWalkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(GetWorldPosition(node), new Vector3(_nodeSize * 0.9f, _nodeSize * 0.9f));
            }
        }
    }
    #endregion
}
