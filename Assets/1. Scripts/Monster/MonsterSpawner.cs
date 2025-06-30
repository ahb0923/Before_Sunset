using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoSingleton<MonsterSpawner>
{
    [Header("# Map Setting")]
    [SerializeField] private Vector2 _mapSize;
    [SerializeField] private int _nodeSize = 1;
    [SerializeField] private LayerMask _obstacleMask;
    public Core Core { get; private set; }

    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private float _spawnTime;

    [Header("# Test")]
    public int spawnCount;
    public int id;
    
    protected override void Awake()
    {
        base.Awake();

        if(Core == null)
        {
            Core = FindObjectOfType<Core>();

            if(Core == null)
            {
                Debug.LogError("[MonsterSpawner] TestCore를 찾지 못했습니다.");
            }
        }

        AstarAlgorithm.GenerateNode(transform.position, _mapSize, _nodeSize, _obstacleMask);
    }

    private void Update()
    {
        AstarAlgorithm.UpdateAllWalkable();
    }

    /// <summary>
    /// 스테이지에 따른 모든 몬스터 소환
    /// </summary>
    [ContextMenu("몬스터 소환")]
    public void SpawnAllMonsters()
    {
        StartCoroutine(C_SpawnMonsters());
    }
    private IEnumerator C_SpawnMonsters()
    {
        int count = 0;
        float timer = 0f;

        while (true)
        {
            if(count >= spawnCount) // test용
                yield break;

            timer += Time.deltaTime;
            if(timer >= _spawnTime)
            {
                timer = 0f;
                count++;

                // 지금은 일단 스테이지마다 무슨 몬스터를 몇 마리 소환할 지 모르니까 Walker만 소환
                SpawnMonster();
            }

            yield return null;
        }
    }
    private void SpawnMonster()
    {
        int randInt = Random.Range(0, _spawnPoints.Count);
        Vector3 pos = _spawnPoints[randInt].position;

        GameObject obj = PoolManager.Instance.GetFromPool(id);
        obj.transform.position = pos;
    }

    /// <summary>
    /// 맵 사이즈 확인
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _mapSize);
    }
}
