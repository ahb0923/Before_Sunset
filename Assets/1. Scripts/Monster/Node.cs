using UnityEngine;

public class Node
{
    public Node parent;
    public int GCost; // 시작 노드에서 현재 노드까지의 거리 비용
    public int HCost; // 목표 노드까지의 예상 거리 비용 (직선 10 / 대각선 14)
    public int FCost => GCost + HCost;

    public bool isWalkable;
    public Vector2Int gridIndex;

    public Node(bool isWalkable, Vector2Int gridIndex)
    {
        this.isWalkable = isWalkable;
        this.gridIndex = gridIndex;
    }
}
