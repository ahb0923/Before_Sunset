using System.Collections.Generic;
using UnityEngine;

public static class AstarAlgorithm
{
    // 경로 찾을 때마다 그리드 만들면 성능에 부하가 걸리므로, 스택 사용
    private static Stack<ANode[,]> _aGridStack = new Stack<ANode[,]>();

    #region Binding Method
    private static NodeGrid _bindGrid;

    /// <summary>
    /// 그리드 바인딩
    /// </summary>
    public static void BindGrid(NodeGrid grid)
    {
        _bindGrid = grid;
    }

    /// <summary>
    /// 시작 노드부터 목표 주변 노드까지의 가장 빠른 경로를 반환<br/>
    /// ※ 목표 노드가 NonWalkable할 때 사용<br/>
    /// ※ 그리드가 바인딩되어 있을 때만 사용
    /// </summary>
    /// <param name="start">시작 노드</param>
    /// <param name="entitySize">경로를 따라가는 엔티티 사이즈 (무조건 홀수 : 1x1이면 1)</param>
    /// <param name="target">타겟 노드</param>
    /// <param name="targetIndex">타겟 인덱스</param>
    /// <param name="targetSize">타겟의 사이즈 (무조건 홀수 : 1x1이면 1)</param>
    /// <param name="canMoveDiagonal">대각선 이동 가능 유무</param>
    public static NodePath FindPathToTarget_Bind(Node start, int entitySize, Node target, int targetIndex, int targetSize, bool canMoveDiagonal = false)
    {
        if (entitySize % 2 == 0 || targetSize % 2 == 0)
        {
            Debug.LogError("[A*] 잘못된 사이즈를 입력했습니다.");
            return null;
        }

        if (_bindGrid == null)
        {
            Debug.LogError("[A*] 그리드가 바인딩되어 있지 않습니다.");
            return null;
        }

        // 초기 세팅 : A* 기반 노드 그리드 생성
        NodeGrid grid = _bindGrid;
        ANode[,] aGrid = CloneNodeGridToANodeGrid(grid, start, target, out ANode startNode, out ANode targetNode);
        startNode.gCost = 0;
        startNode.hCost = GetHeuristicDistance(grid, start, target);

        // 초기 세팅 : 리스트 & 셋 초기화
        List<ANode> openList = new List<ANode>();
        HashSet<ANode> closedSet = new HashSet<ANode>();

        // 초기 세팅 : 오픈 리스트에 시작 노드 추가
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // F 비용이 가장 낮은 노드 → H 비용이 가장 낮은 노드 순으로 탐색
            ANode curNode = openList[0];
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
            if (curNode.ActualNode == target)
            {
                _aGridStack.Push(aGrid);
                return RetracePath(grid, startNode, targetNode);
            }

            foreach (ANode neighbor in GetNeighbors(grid, aGrid, curNode.ActualNode, canMoveDiagonal))
            {
                // 클로즈 셋에 포함되어 있으면 넘김
                if (closedSet.Contains(neighbor)) continue;

                // 엔티티 사이즈에 따라 NonWalkable이면 넘김
                if (!IsAreaWalkable(grid, neighbor.ActualNode, entitySize, targetIndex)) continue;

                // 검색해야 할 이웃 노드 오픈 리스트에 추가 & 코스트와 부모 노드 업데이트
                int updatedGCost = curNode.gCost + GetHeuristicDistance(grid, curNode.ActualNode, neighbor.ActualNode);
                if (updatedGCost < neighbor.gCost)
                {
                    neighbor.parent = curNode;
                    neighbor.gCost = updatedGCost;
                    neighbor.hCost = GetHeuristicDistance(grid, neighbor.ActualNode, target);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        _aGridStack.Push(aGrid);
        return null;
    }

    /// <summary>
    /// 엔티티 사이즈에 따른 이동 불가 유무 반환<br/>
    /// ※ 그리드가 바인딩되어 있을 때만 사용
    /// </summary>
    public static bool IsAreaWalkable_Bind(Node center, int entitySize, int targetIndex)
    {
        if (_bindGrid == null)
        {
            Debug.LogError("[A*] 그리드가 바인딩되어 있지 않습니다.");
            return false;
        }

        NodeGrid grid = _bindGrid;
        if (!grid.HasNode(center)) return false;

        Vector2Int gridIndex = grid.GetGridIndex(center);
        int boundary = entitySize / 2;

        for (int x = -boundary; x <= boundary; x++)
        {
            for (int y = -boundary; y <= boundary; y++)
            {
                int gridX = gridIndex.x + x;
                int gridY = gridIndex.y + y;

                if (!grid.IsValidIndex(gridX, gridY)) continue;
                if (!grid.Nodes[gridX, gridY].IsWalkable(targetIndex)) return false;
            }
        }

        return true;
    }
    #endregion

    /// <summary>
    /// 시작 노드부터 목표 주변 노드까지의 가장 빠른 경로를 반환<br/>
    /// ※ 목표 노드가 NonWalkable할 때 사용
    /// </summary>
    /// <param name="grid">맵 상의 노드 그리드</param>
    /// <param name="start">시작 노드</param>
    /// <param name="entitySize">경로를 따라가는 엔티티 사이즈 (무조건 홀수 : 1x1이면 1)</param>
    /// <param name="target">타겟 노드</param>
    /// <param name="targetIndex">타겟 인덱스</param>
    /// <param name="targetSize">타겟의 사이즈 (무조건 홀수 : 1x1이면 1)</param>
    /// <param name="canMoveDiagonal">대각선 이동 가능 유무</param>
    public static NodePath FindPathToTarget(NodeGrid grid, Node start, int entitySize , Node target, int targetIndex, int targetSize, bool canMoveDiagonal = false)
    {
        if (entitySize % 2 == 0 || targetSize % 2 == 0)
        {
            Debug.LogError("[A*] 잘못된 사이즈를 입력했습니다.");
            return null;
        }

        // 초기 세팅 : A* 기반 노드 그리드 생성
        ANode[,] aGrid = CloneNodeGridToANodeGrid(grid, start, target, out ANode startNode, out ANode targetNode);
        startNode.gCost = 0;
        startNode.hCost = GetHeuristicDistance(grid, start, target);

        // 초기 세팅 : 리스트 & 셋 초기화
        List<ANode> openList = new List<ANode>();
        HashSet<ANode> closedSet = new HashSet<ANode>();

        // 초기 세팅 : 오픈 리스트에 시작 노드 추가
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // F 비용이 가장 낮은 노드 → H 비용이 가장 낮은 노드 순으로 탐색
            ANode curNode = openList[0];
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
            if (curNode.ActualNode == target)
            {
                _aGridStack.Push(aGrid);
                return RetracePath(grid, startNode, targetNode);
            }

            foreach (ANode neighbor in GetNeighbors(grid, aGrid, curNode.ActualNode, canMoveDiagonal))
            {
                // 클로즈 셋에 포함되어 있으면 넘김
                if (closedSet.Contains(neighbor)) continue;

                // 엔티티 사이즈에 따라 NonWalkable이면 넘김
                if (!IsAreaWalkable(grid, neighbor.ActualNode, entitySize, targetIndex)) continue;

                // 검색해야 할 이웃 노드 오픈 리스트에 추가 & 코스트와 부모 노드 업데이트
                int updatedGCost = curNode.gCost + GetHeuristicDistance(grid, curNode.ActualNode, neighbor.ActualNode);
                if (updatedGCost < neighbor.gCost)
                {
                    neighbor.parent = curNode;
                    neighbor.gCost = updatedGCost;
                    neighbor.hCost = GetHeuristicDistance(grid, neighbor.ActualNode, target);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        _aGridStack.Push(aGrid);
        return null;
    }

    /// <summary>
    /// 실제 노드 그리드 기반으로 A* 노드 그리드를 생성
    /// </summary>
    private static ANode[,] CloneNodeGridToANodeGrid(NodeGrid grid, Node start, Node target, out ANode startNode, out ANode targetNode)
    {
        startNode = null;
        targetNode = null;
        int xCount = grid.XCount;
        int yCount = grid.YCount;

        ANode[,] aGrid;
        if (_aGridStack.Count > 0)
        {
            aGrid = _aGridStack.Pop();

            if(aGrid.GetLength(0) == xCount && aGrid.GetLength(1) == yCount)
            {
                foreach (ANode aNode in aGrid)
                {
                    aNode.Init();
                }
            }
        }
        else
        {
            aGrid = new ANode[xCount, yCount];

            for (int x = 0; x < xCount; x++)
            {
                for (int y = 0; y < yCount; y++)
                {
                    aGrid[x, y] = new ANode(grid.Nodes[x, y]);
                }
            }
        }

        Vector2Int startIndex = grid.GetGridIndex(start);
        Vector2Int targetIndex = grid.GetGridIndex(target);
        startNode = aGrid[startIndex.x, startIndex.y];
        targetNode = aGrid[targetIndex.x, targetIndex.y];

        return aGrid;
    }

    /// <summary>
    /// 노드 사이의 휴리스틱 거리를 반환<br/>
    /// ※ 상하좌우 10 / 대각선 14
    /// </summary>
    private static int GetHeuristicDistance(NodeGrid grid, Node a, Node b)
    {
        if(!grid.HasNode(a) || !grid.HasNode(b))
        {
            Debug.LogError("[A*] 노드가 그리드에 포함되어 있지 않습니다.");
            return -1;
        }

        int distX = Mathf.Abs(grid.GetGridIndex(a).x - grid.GetGridIndex(b).x);
        int distY = Mathf.Abs(grid.GetGridIndex(a).y - grid.GetGridIndex(b).y);

        return 14 * Mathf.Min(distX, distY) + 10 * Mathf.Abs(distX - distY);
    }

    /// <summary>
    /// 목표 노드에서 부모 노드를 역추적해서 시작 노드까지의 경로를 반환
    /// </summary>
    private static NodePath RetracePath(NodeGrid grid, ANode startNode, ANode endNode)
    {
        List<Node> path = new List<Node>();
        ANode current = endNode;

        while (current != startNode)
        {
            path.Add(current.ActualNode);
            current = current.parent;
        }
        path.Add(startNode.ActualNode);
        path.Reverse();

        return new NodePath(grid, startNode.ActualNode, endNode.ActualNode, path);
    }

    /// <summary>
    /// 이웃 노드 리스트를 반환<br/>
    /// ※ containDiagonal : 대각선 포함 유무
    /// </summary>
    private static List<ANode> GetNeighbors(NodeGrid grid, ANode[,] aGrid, Node center, bool canMoveDiagonal)
    {
        if (!grid.HasNode(center)) return null;

        Vector2Int gridIndex = grid.GetGridIndex(center);

        List<ANode> neighborList = new List<ANode>();
        for (int x = gridIndex.x - 1; x <= gridIndex.x + 1; x++)
        {
            for (int y = gridIndex.y - 1; y <= gridIndex.y + 1; y++)
            {
                if (!grid.IsValidIndex(x, y)) continue; // 범위 밖 제외
                if (x == gridIndex.x && y == gridIndex.y) continue; // 자기 자신 제외
                if (!canMoveDiagonal && Mathf.Abs(x - gridIndex.x) == 1 && Mathf.Abs(y - gridIndex.y) == 1) continue; // 대각선 제외

                neighborList.Add(aGrid[x, y]);
            }
        }

        return neighborList;
    }

    /// <summary>
    /// 엔티티 사이즈에 따른 이동 불가 유무 반환
    /// </summary>
    public static bool IsAreaWalkable(NodeGrid grid, Node center, int entitySize, int targetIndex)
    {
        if (!grid.HasNode(center)) return false;

        Vector2Int gridIndex = grid.GetGridIndex(center);
        int boundary = entitySize / 2;

        for (int x = -boundary; x <= boundary; x++)
        {
            for (int y = -boundary; y <= boundary; y++)
            {
                int gridX = gridIndex.x + x;
                int gridY = gridIndex.y + y;

                if (!grid.IsValidIndex(gridX, gridY)) continue;
                if (!grid.Nodes[gridX, gridY].IsWalkable(targetIndex)) return false;
            }
        }

        return true;
    }
}

// 실제 위치 정보를 가지고 있는 노드
public class Node
{
    public Vector3 WorldPos { get; private set; }
    public int walkableIndex;

    public Node(Vector3 worldPos)
    {
        WorldPos = worldPos;
        walkableIndex = -1;
    }

    public Node(Vector3 worldPos, int index)
    {
        WorldPos = worldPos;
        walkableIndex = index;
    }

    public bool IsWalkable(int index)
    {
        // -1 : 무조건 walkable
        if (walkableIndex == -1) return true;

        // index와 동일 : 이동하려는 타겟이므로 true
        if (walkableIndex == index) return true;

        return false;
    }
}

// A* 계산을 위한 가상의 노드
public class ANode
{
    public Node ActualNode { get; private set; }

    public ANode parent;
    public int gCost; // 시작 노드에서 현재 노드까지의 거리 비용
    public int hCost; // 목표 노드까지의 예상 거리 비용 (직선 10 / 대각선 14)
    public int FCost => gCost + hCost;

    public ANode(Node node)
    {
        ActualNode = node;
        gCost = int.MaxValue;
        parent = null;
    }

    public void Init()
    {
        gCost = int.MaxValue;
        parent = null;
    }
}

// 노드 그리드
public class NodeGrid
{
    private int _nodeSize;
    private float _nodeHalfSize => _nodeSize * 0.5f;
    private Vector3 _bottomLeft;
    private Dictionary<Node, Vector2Int> _nodeDict;

    public int XCount { get; private set; }
    public int YCount { get; private set; }
    public Node[,] Nodes { get; private set; }

    /// <summary>
    /// 노드 그리드 생성자
    /// </summary>
    /// <param name="center">노드 그리드 정중앙</param>
    /// <param name="mapSize">맵 사이즈</param>
    /// <param name="nodeSize">노드 사이즈 (입력 안 하면 1)</param>
    public NodeGrid(Vector3 center, Vector2 mapSize, int nodeSize = 1)
    {
        _nodeSize = nodeSize;
        _bottomLeft = center - new Vector3(mapSize.x * 0.5f, mapSize.y * 0.5f);
        _nodeDict = new Dictionary<Node, Vector2Int>();

        XCount = Mathf.CeilToInt(mapSize.x / _nodeSize);
        YCount = Mathf.CeilToInt(mapSize.y / _nodeSize);
        Nodes = new Node[XCount, YCount];

        for (int x = 0; x < XCount; x++)
        {
            for (int y = 0; y < YCount; y++)
            {
                Vector3 worldPos = _bottomLeft + new Vector3(x * nodeSize + _nodeHalfSize, y * nodeSize + _nodeHalfSize);
                Node node = new Node(worldPos);
                Nodes[x, y] = node;
                _nodeDict[node] = new Vector2Int(x, y);
            }
        }
    }

    /// <summary>
    /// 월드 포지션에서 가장 가까운 노드 반환<br/>
    /// ※ 맵을 벗어나는 위치 값에 경우 null 반환
    /// </summary>
    /// <param name="worldPos">월드 포지션</param>
    public Node GetNode(Vector3 worldPos)
    {
        Vector2Int gridIndex = GetGridIndex(worldPos);

        if (gridIndex.x == -1)
            return null;
        else
            return Nodes[gridIndex.x, gridIndex.y];
    }

    /// <summary>
    /// 매개변수와 가장 가까운 노드의 그리드 인덱스 반환<br/>
    /// ※ 맵을 벗어나거나 포함되지 않은 경우 (-1, -1) 반환
    /// </summary>
    public Vector2Int GetGridIndex(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - _bottomLeft;

        int x = Mathf.FloorToInt(localPos.x / _nodeSize);
        int y = Mathf.FloorToInt(localPos.y / _nodeSize);

        if (IsValidIndex(x, y))
            return new Vector2Int(x, y);
        else
            return Vector2Int.one * -1;
    }
    public Vector2Int GetGridIndex(Node node)
    {
        if (!HasNode(node)) return Vector2Int.one * -1;

        return _nodeDict[node];
    }

    /// <summary>
    /// 오브젝트 포지션과 사이즈에 따른 노드들의 Walkable Index 세팅
    /// </summary>
    /// <param name="index">세팅할 인덱스</param>
    /// <param name="worldPos">오브젝트 월드 포지션</param>
    /// <param name="size">오브젝트 사이즈 (1x1이면 1)</param>
    public void SetWalkableIndex(int index, Vector3 worldPos, int size)
    {
        Vector2Int center = GetGridIndex(worldPos);
        if (center.x == -1) 
        {
            Debug.LogWarning("[NodeGrid] 해당 오브젝트는 맵 바깥에 있습니다.");
            return;
        }

        int boundary = size / 2;
        for (int x = center.x - boundary; x <= center.x + boundary; x++) 
        {
            for (int y = center.y - boundary; y <= center.y + boundary; y++) 
            {
                if (!IsValidIndex(x, y)) continue;

                Nodes[x, y].walkableIndex = index;
            }
        }
    }

    /// <summary>
    /// 그리드 범위를 벗어나는지 체크
    /// </summary>
    public bool IsValidIndex(int x, int y)
    {
        return 0 <= x && x < XCount && 0 <= y && y < YCount;
    }
    public bool IsValidIndex(Vector2Int gridIndx)
    {
        return IsValidIndex(gridIndx.x, gridIndx.y);
    }

    /// <summary>
    /// 노드 그리드에 해당 노드가 포함되는지 체크
    /// </summary>
    public bool HasNode(Node node)
    {
        return _nodeDict.ContainsKey(node);
    }
}

// 경로
public class NodePath
{
    public NodeGrid Grid { get; private set; }
    public Node StartNode { get; private set; }
    public Node EndNode { get; private set; }
    public List<Node> Path { get; private set; }

    public Node CurNode => Path[_index];
    private int _index;

    public NodePath(NodeGrid grid, Node startNode, Node endNode, List<Node> path)
    {
        Grid = grid;
        StartNode = startNode;
        EndNode = endNode;
        Path = path;

        _index = 0;
    }

    public NodePath(NodePath nodePath)
    {
        Grid = nodePath.Grid;
        StartNode = nodePath.StartNode;
        EndNode = nodePath.EndNode;
        Path = nodePath.Path;

        _index = 0;
    }

    /// <summary>
    /// CurNode를 경로의 다음 노드로 이동
    /// </summary>
    public bool Next()
    {
        if (_index + 1 >= Path.Count)
        {
            return false;
        }

        _index++;
        return true;
    }

    /// <summary>
    /// 남은 경로의 이동 가능 유무를 조사하여 갈 수 있는 경로인지 판단
    /// </summary>
    public bool IsWalkablePath(int entitySize, int targetIndex)
    {
        for (int i = _index; i < Path.Count; i++)
        {
            if (!AstarAlgorithm.IsAreaWalkable(Grid, Path[i], entitySize, targetIndex))
            {
                return false;
            }
        }

        return true;
    }
}