using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnData
{
    public int id;
    public int spawnCount;
}

public class MonsterSpawner : MonoBehaviour
{
    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private float _spawnTime;
    [SerializeField] private Transform _monsterParent;

    [Header("# Test")]
    [SerializeField] private List<SpawnData> _spawnDatas;

    /// <summary>
    /// 스테이지에 따른 모든 몬스터 소환
    /// </summary>
    public void SpawnAllMonsters(int index)
    {
        if (index < 0 || index >= _spawnDatas.Count)
        {
            Debug.LogError("[MonsterSpawner] 잘못된 스폰 데이터 인덱스를 입력했습니다.");
            return;
        }

        StartCoroutine(C_SpawnMonsters(index));
    }

    /// <summary>
    /// 스폰 타임마다 몬스터를 소환하는 코루틴
    /// </summary>
    private IEnumerator C_SpawnMonsters(int spawnDataNum)
    {
        SpawnData spawnData = _spawnDatas[spawnDataNum];

        int count = 0;
        float timer = 0f;

        while (true)
        {
            if(count >= spawnData.spawnCount)
                yield break;

            timer += Time.deltaTime;
            if(timer >= _spawnTime)
            {
                timer = 0f;
                count++;

                SpawnMonster(spawnData.id);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 해당하는 몬스터 ID를 가진 몬스터 소환
    /// </summary>
    private void SpawnMonster(int monsterId)
    {
        Vector3 pos = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
        GameObject obj = PoolManager.Instance.GetFromPool(monsterId, pos, _monsterParent);
    }
}
