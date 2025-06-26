using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node parent;
    public int gCost; // 시작 노드에서 현재 노드까지의 거리 비용
    public int hCost; // 목표 노드까지의 예상 거리 비용 (직선 10 / 대각선 14)
    public int FCost => gCost + hCost;
    public bool isWalkable;

    public Vector2Int GridIndex { get; private set; }
    public Vector3 WorldPos { get; private set; }

    public Node(bool isWalkable, Vector2Int gridIndex, Vector3 worldPos)
    {
        this.isWalkable = isWalkable;
        GridIndex = gridIndex;
        WorldPos = worldPos;
    }
}

public static class AstarAlgorithm
{
    /// <summary>
    /// 해당 노드 그리드에서 시작 노드에서 목표 노드까지의 길(노드 리스트)을 반환
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <param name="containDiagonalNode"></param>
    public static List<Node> FindPath(Node[,] grid, Node startNode, Node endNode, int entitySize, bool containDiagonalNode = false)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // 모든 노드 초기화
        foreach (Node node in grid)
        {
            node.gCost = int.MaxValue;
            node.parent = null;
        }

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
            if (curNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (Node neighbor in GetNeighbors(grid, curNode, containDiagonalNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

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
    /// 노드 사이의 휴리스틱 거리를 반환
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static int GetHeuristicDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.GridIndex.x - b.GridIndex.x);
        int distY = Mathf.Abs(a.GridIndex.y - b.GridIndex.y);

        return 14 * Mathf.Min(distX, distY) + 10 * Mathf.Abs(distX - distY);
    }

    /// <summary>
    /// node의 이웃 노드 리스트를 반환
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="node"></param>
    /// <param name="containDiagonal">대각선 노드 포함을 결정</param>
    /// <returns></returns>
    private static List<Node> GetNeighbors(Node[,] grid, Node node, bool containDiagonal)
    {
        List<Node> neighborList = new List<Node>();
        int maxX = grid.GetLength(0);
        int maxY = grid.GetLength(1);

        for (int x = node.GridIndex.x - 1; x <= node.GridIndex.x + 1; x++)
        {
            for (int y = node.GridIndex.y - 1; y <= node.GridIndex.y + 1; y++)
            {
                if (x < 0 || y < 0 || x >= maxX || y >= maxY) continue; // 범위 밖 제외
                if (!node.isWalkable) continue; // 해당 노드로 갈 수 없으면 제외
                if (x == node.GridIndex.x && y == node.GridIndex.y) continue; // 자기 자신 제외
                if (Mathf.Abs(x - node.GridIndex.x) == 1 && Mathf.Abs(y - node.GridIndex.y) == 1 && !containDiagonal) continue; // 대각선 제외

                neighborList.Add(grid[x, y]);
            }
        }

        return neighborList;
    }

    /// <summary>
    /// 목표 노드부터 부모 노드를 역추적해서 시작 노드까지의 길을 반환
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns></returns>
    private static List<Node> RetracePath(Node startNode, Node endNode)
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
}
