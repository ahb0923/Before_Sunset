using UnityEngine;

public class TestMonster : MonoBehaviour
{
    private Transform _testCore;
    [SerializeField] private string _testCoreName = "TestCore";
    [SerializeField] private float _speed = 3f;

    private void Awake()
    {
        _testCore = GameObject.Find(_testCoreName)?.GetComponent<Transform>();
        if(_testCore == null)
        {
            throw new System.NullReferenceException("TestCore가 현재 씬에 없습니다.");
        }
    }

    private void Update()
    {
        if (_testCore == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _testCore.position, _speed * Time.deltaTime);
    }
}
