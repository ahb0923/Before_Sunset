using System.Collections.Generic;
using UnityEngine;

public static class AstarAlgorithm
{
    private static int _nodeSize;
    private static float _nodeHalfSize => _nodeSize * 0.5f;
    private static bool _containDiagonal;
    private static Node[,] _grid;
    private static Dictionary<Node, Vector2Int> _gridIndexDict;
    private static Vector3 _bottomLeftNodePos;
    private static LayerMask _obstacleMask;
    private static bool _isInitialized;

    private static Node _coreNode;
    private static Dictionary<(Node, int), NodePath> _pathCacheToCoreDict;
    private static HashSet<Node> _coreNodeSet;
    private static bool _isSetCore;

    /// <summary>
    /// 맵과 노드 사이즈에 맞는 노드들을 생성함<br/>
    /// 모든 A* 메서드는 노드 생성 후에 사용 가능
    /// </summary>
    public static void GenerateNodeFromTarget(Vector3 center, Vector2 mapSize, int nodeSize, bool containDiagonal, LayerMask obstacleMask)
    {
        _nodeSize = nodeSize;
        _containDiagonal = containDiagonal;
        int nodeCountX = Mathf.CeilToInt(mapSize.x / _nodeSize);
        int nodeCountY = Mathf.CeilToInt(mapSize.y / _nodeSize);
        _grid = new Node[nodeCountX, nodeCountY];
        _gridIndexDict = new Dictionary<Node, Vector2Int>();

        Vector3 halfMapSize = new Vector3(mapSize.x * 0.5f, mapSize.y * 0.5f);
        _bottomLeftNodePos = center - halfMapSize + new Vector3(_nodeHalfSize, _nodeHalfSize);
        _obstacleMask = obstacleMask;

        for (int x = 0; x < nodeCountX; x++)
        {
            for (int y = 0; y < nodeCountY; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = _bottomLeftNodePos + new Vector3(x * _nodeSize, y * _nodeSize);
                Collider2D hit = Physics2D.OverlapBox(worldPos, new Vector2(_nodeHalfSize, _nodeHalfSize), 0, _obstacleMask);

                Node node = new Node(hit == null, worldPos);
                _grid[x, y] = node;
                _gridIndexDict[node] = gridPos;
            }
        }

        _isInitialized = true;
    }

    /// <summary>
    /// 코어 설정<br/>
    /// FindPathToCore 메서드는 코어 설정 후에 가능
    /// </summary>
    public static void SetCore(Transform core, int coreSize)
    {
        _coreNode = GetNodeFromWorldPosition(core.position);
        _coreNodeSet = GetSurroudingNodesFromTarget(_coreNode, coreSize, true);
        _pathCacheToCoreDict = new Dictionary<(Node, int), NodePath>();

        _isSetCore = true;
    }

    /// <summary>
    /// 노드 그리드 상에서 시작 노드부터 설정한 코어까지의 가장 빠른 경로를 반환
    /// </summary>
    public static NodePath FindPathToCore(Node startNode, int entitySize)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[A*] 노드 그리드가 생성되어 있지 않습니다.");
            return null;
        }

        if (!_isSetCore)
        {
            Debug.LogError("[A*] 코어가 설정되어 있지 않습니다.");
            return null;
        }

        // 캐싱된 경로가 있으면 반환
        if (_pathCacheToCoreDict.TryGetValue((startNode, entitySize), out var pathCache) && pathCache.IsWalkablePath(entitySize))
        {
            return new NodePath(pathCache);
        }

        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // 모든 노드 초기화
        foreach (Node node in _grid)
        {
            node.gCost = int.MaxValue;
            node.parent = null;
        }

        // 시작 노드 초기화
        startNode.gCost = 0;
        startNode.hCost = GetHeuristicDistance(startNode, _coreNode);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // F 비용이 가장 낮은 노드 → H 비용이 가장 낮은 노드 순으로 탐색
            Node curNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < curNode.FCost ||
                   (openList[i].FCost == curNode.FCost && openList[i].hCost < curNode.hCost))
                {
                    curNode = openList[i];
                }
            }

            // F 비용이 가장 낮은 노드는 오픈 리스트에서 제거 & 클로즈 셋에 추가
            openList.Remove(curNode);
            closedSet.Add(curNode);

            // 목표 노드에 도달했을 경우, 길을 역추적하여 반환
            if (_coreNodeSet.Contains(curNode))
            {
                NodePath path = RetracePath(startNode, curNode);
                _pathCacheToCoreDict[(startNode, entitySize)] = path;
                return path;
            }

            foreach (Node neighbor in GetNeighbors(curNode, entitySize, _containDiagonal))
            {
                if (closedSet.Contains(neighbor)) continue;

                // 검색해야 할 이웃 노드 오픈 리스트에 추가 & 코스트와 부모 노드 업데이트
                int updatedGCost = curNode.gCost + GetHeuristicDistance(curNode, neighbor);
                if (updatedGCost < neighbor.gCost)
                {
                    neighbor.parent = curNode;
                    neighbor.gCost = updatedGCost;
                    neighbor.hCost = GetHeuristicDistance(neighbor, _coreNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 노드 그리드 상에서 시작 노드부터 목표 노드까지의 가장 빠른 경로를 반환
    /// </summary>
    public static NodePath FindPath(Node startNode, int entitySize ,Transform target, int targetSize)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[A*] 노드 그리드가 생성되어 있지 않습니다.");
            return null;
        }

        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // 모든 노드 초기화
        foreach (Node node in _grid)
        {
            node.gCost = int.MaxValue;
            node.parent = null;
        }

        // 엔드 노드 초기화
        Node endNode = GetNodeFromWorldPosition(target.position);
        HashSet<Node> endNodeSet = GetSurroudingNodesFromTarget(endNode, targetSize);

        // 시작 노드 초기화
        startNode.gCost = 0;
        startNode.hCost = GetHeuristicDistance(startNode, endNode);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // F 비용이 가장 낮은 노드 → H 비용이 가장 낮은 노드 순으로 탐색
            Node curNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < curNode.FCost ||
                   (openList[i].FCost == curNode.FCost && openList[i].hCost < curNode.hCost))
                {
                    curNode = openList[i];
                }
            }

            // F 비용이 가장 낮은 노드는 오픈 리스트에서 제거 & 클로즈 셋에 추가
            openList.Remove(curNode);
            closedSet.Add(curNode);

            // 목표 노드에 도달했을 경우, 길을 역추적하여 반환
            if (endNodeSet.Contains(curNode))
            {
                NodePath path = RetracePath(startNode, curNode);
                return path;
            }

            foreach (Node neighbor in GetNeighbors(curNode, entitySize, _containDiagonal))
            {
                if (closedSet.Contains(neighbor)) continue;

                // 검색해야 할 이웃 노드 오픈 리스트에 추가 & 코스트와 부모 노드 업데이트
                int updatedGCost = curNode.gCost + GetHeuristicDistance(curNode, neighbor);
                if (updatedGCost < neighbor.gCost)
                {
                    neighbor.parent = curNode;
                    neighbor.gCost = updatedGCost;
                    neighbor.hCost = GetHeuristicDistance(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 월드 포지션에 대한 가장 가까운 노드를 반환
    /// </summary>
    public static Node GetNodeFromWorldPosition(Vector3 worldPos)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[A*] 노드 그리드가 생성되어 있지 않습니다.");
            return null;
        }

        Vector3 relative = worldPos - _bottomLeftNodePos;

        int x = Mathf.RoundToInt(relative.x / _nodeSize);
        int y = Mathf.RoundToInt(relative.y / _nodeSize);

        // 맵 바깥이면, null 반환
        if (x < 0 || y < 0 || x >= _grid.GetLength(0) || y >= _grid.GetLength(1))
            return null;

        return _grid[x, y];
    }

    /// <summary>
    /// 모든 노드의 이동 가능 유무를 업데이트
    /// </summary>
    public static void UpdateAllWalkable()
    {
        foreach (Node node in _grid)
        {
            UpdateWalkable(node);
        }
    }

    /// <summary>
    /// 해당 노드의 이동 가능 유무를 업데이트
    /// </summary>
    public static void UpdateWalkable(Node node)
    {
        Collider2D hit = Physics2D.OverlapBox(node.WorldPos, new Vector2(_nodeHalfSize, _nodeHalfSize), 0, _obstacleMask);
        node.isWalkable = hit == null;
    }

    /// <summary>
    /// 엔티티 사이즈에 따른 이동 불가 유무 반환
    /// </summary>
    public static bool IsAreaWalkable(Node center, int entitySize)
    {
        Vector2Int gridIndex = _gridIndexDict[center];
        int boundary = entitySize / 2;
        int maxX = _grid.GetLength(0);
        int maxY = _grid.GetLength(1);

        for (int x = -boundary; x <= boundary; x++)
        {
            for (int y = -boundary; y <= boundary; y++)
            {
                int gridX = gridIndex.x + x;
                int gridY = gridIndex.y + y;

                if (gridX < 0 || gridY < 0 || gridX >= maxX || gridY >= maxY) continue;
                if (_grid[gridX, gridY].isCore) continue;
                if (!_grid[gridX, gridY].isWalkable) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 노드 사이의 휴리스틱 거리를 반환<br/>
    /// ※ 상하좌우 10 / 대각선 14
    /// </summary>
    private static int GetHeuristicDistance(Node a, Node b)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[A*] 노드 그리드가 생성되어 있지 않습니다.");
            return -1;
        }

        int distX = Mathf.Abs(_gridIndexDict[a].x - _gridIndexDict[b].x);
        int distY = Mathf.Abs(_gridIndexDict[a].y - _gridIndexDict[b].y);

        return 14 * Mathf.Min(distX, distY) + 10 * Mathf.Abs(distX - distY);
    }

    /// <summary>
    /// 이웃 노드 리스트를 반환<br/>
    /// ※ containDiagonal : 대각선 포함 유무
    /// </summary>
    private static List<Node> GetNeighbors(Node node, int entitySize, bool containDiabonal)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[A*] 노드 그리드가 생성되어 있지 않습니다.");
            return null;
        }

        if(entitySize == 0)
        {
            Debug.LogError("[A*] 엔티티 사이즈가 0 입니다.");
            return null;
        }

        List<Node> neighborList = new List<Node>();
        int maxX = _grid.GetLength(0);
        int maxY = _grid.GetLength(1);

        for (int x = _gridIndexDict[node].x - 1; x <= _gridIndexDict[node].x + 1; x++)
        {
            for (int y = _gridIndexDict[node].y - 1; y <= _gridIndexDict[node].y + 1; y++)
            {
                if (x < 0 || y < 0 || x >= maxX || y >= maxY) continue; // 범위 밖 제외
                if (x == _gridIndexDict[node].x && y == _gridIndexDict[node].y) continue; // 자기 자신 제외
                if (!containDiabonal && Mathf.Abs(x - _gridIndexDict[node].x) == 1 && Mathf.Abs(y - _gridIndexDict[node].y) == 1) continue; // 대각선 제외

                if (!IsAreaWalkable(_grid[x, y], entitySize)) continue; // 이동 불가능하면 제외

                neighborList.Add(_grid[x, y]);
            }
        }

        return neighborList;
    }

    /// <summary>
    /// 목표 노드에서 부모 노드를 역추적해서 시작 노드까지의 경로를 반환
    /// </summary>
    private static NodePath RetracePath(Node startNode, Node endNode)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[A*] 노드 그리드가 생성되어 있지 않습니다.");
            return null;
        }

        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Add(startNode);
        path.Reverse();

        return new NodePath(startNode, endNode, path);
    }

    /// <summary>
    /// 타겟 근처의 모든 노드 해시셋를 반환
    /// </summary>
    private static HashSet<Node> GetSurroudingNodesFromTarget(Node targetNode, int targetSize, bool isCore = false)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[A*] 노드 그리드가 생성되어 있지 않습니다.");
            return null;
        }

        Vector2Int targetGridIndex = _gridIndexDict[targetNode];
        HashSet<Node> nodes = new HashSet<Node>();

        int boundary = targetSize / 2 + 1;
        for (int x = -boundary; x <= boundary; x++)
        {
            for (int y = -boundary; y <= boundary; y++)
            {
                int gridX = targetGridIndex.x + x;
                int gridY = targetGridIndex.y + y;

                if(Mathf.Abs(x) == boundary || Mathf.Abs(y) == boundary)
                {
                    nodes.Add(_grid[gridX, gridY]);
                }
                else if(isCore)
                {
                    _grid[gridX, gridY].isCore = true;
                }
            }
        }
        
        return nodes;
    }
}
