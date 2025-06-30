using System.Collections.Generic;

public class NodePath
{
    public Node StartNode {  get; private set; }
    public Node EndNode { get; private set; }

    public List<Node> Path { get; private set; }
    private int _index;

    public Node CurNode => Path[_index];

    public NodePath(Node startNode, Node endNode, List<Node> path)
    {
        StartNode = startNode;
        EndNode = endNode;
        Path = path;

        _index = 0;
    }

    public NodePath(NodePath nodePath)
    {
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
    public bool IsWalkablePath(int entitySize)
    {
        for(int i = _index; i < Path.Count; i++)
        {
            if (!AstarAlgorithm.IsAreaWalkable(Path[i], entitySize))
            {
                return false;
            }
        }

        return true;
    }
}
