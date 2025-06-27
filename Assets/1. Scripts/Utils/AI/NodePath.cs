using System.Collections.Generic;

public class NodePath
{
    public Node StartNode {  get; private set; }
    public Node EndNode { get; private set; }

    private List<Node> _path;
    private int _index;

    public Node CurNode => _path[_index];

    public NodePath(Node startNode, Node endNode, List<Node> path)
    {
        StartNode = startNode;
        EndNode = endNode;
        _path = path;

        _index = 0;
    }

    /// <summary>
    /// CurNode를 경로의 다음 노드로 이동
    /// </summary>
    public bool Next()
    {
        if (_index + 1 >= _path.Count)
        {
            return false;
        }

        _index++;
        return true;
    }

    /// <summary>
    /// 남은 경로의 이동 가능 유무를 조사하여 갈 수 있는 경로인지 판단
    /// </summary>
    public bool IsWalkablePath()
    {
        int index = _path.IndexOf(CurNode);

        for(int i = index; i < _path.Count; i++)
        {
            if (!_path[i].isWalkable)
            {
                return false;
            }
        }

        return true;
    }
}
