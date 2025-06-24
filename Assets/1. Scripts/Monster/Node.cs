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
