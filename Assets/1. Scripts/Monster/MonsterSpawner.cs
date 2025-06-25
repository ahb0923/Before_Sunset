using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnTime;
    private float timer = 0f;

    [SerializeField] private int level; // test용으로 인스펙터에서 받아올 수 있게 함
    private int count = 0;

    private void Update()
    {
        if (count >= level * 20) return; 

        timer += Time.deltaTime;
        if (timer > _spawnTime)
        {
            timer = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObject enemy = PoolManager.Instance.GetFromPool(POOL_TYPE.Monster);
        enemy.transform.position = _spawnPoints[0].position;
        count++;
    }
}
