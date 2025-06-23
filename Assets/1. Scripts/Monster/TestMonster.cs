using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    private Transform _testCore;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private string _testCoreName = "TestCore";
    [SerializeField] private float _speed = 3f;

    private List<Node> _path;

    private void Awake()
    {
        _testCore = GameObject.Find(_testCoreName)?.GetComponent<Transform>();
        if(_testCore == null)
        {
            throw new System.NullReferenceException("TestCore가 현재 씬에 없습니다.");
        }

        _path = new List<Node>();
    }

    private void Update()
    {
        if (_testCore == null || _path == null || _path?.Count <= 0) return;

        Vector3 targetPos = MapManager.Instance.GetWorldPosition(_path[0]);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            _path.RemoveAt(0);
        }
    }

    private void OnDisable()
    {
        _path?.Clear();
    }

    [ContextMenu("길 찾기")]
    private void GeneratePath()
    {
        _startPos = transform.position;
        _path = MapManager.Instance.FindPath(_startPos, _testCore.position);
        if (_path == null)
            Debug.Log("코어로 해당하는 길이 없습니다!");
    }
}
