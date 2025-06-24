using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private float _speed = 3f;

    private List<Node> _path = new List<Node>();

    private void Update()
    {
        if (_path == null) return;
        if (_path.Count == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, _path[0].WorldPos, _speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, _path[0].WorldPos) < 0.01f)
        {
            transform.position = _path[0].WorldPos;
            _path.RemoveAt(0);
        }
    }

    private void OnDisable()
    {
        _path?.Clear();
    }

    [ContextMenu("길 찾기")]
    public void GeneratePath()
    {
        _startPos = transform.position;
        StartCoroutine(MapManager.Instance.C_FindPath(_startPos, MapManager.Instance.TestCore.position, path => _path = path));
        if (_path == null)
            Debug.Log("코어로 해당하는 길이 없습니다!");
    }
}
