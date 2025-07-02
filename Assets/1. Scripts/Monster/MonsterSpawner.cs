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

    private HashSet<BaseMonster> _aliveMonsterSet = new HashSet<BaseMonster>();

    public void OnObstacleDestroyed()
    {
        HashSet<BaseMonster> deadMonsterSet = new HashSet<BaseMonster>();

        foreach (BaseMonster monster in _aliveMonsterSet)
        {
            if (!monster.gameObject.activeInHierarchy)
            {
                deadMonsterSet.Add(monster);
                continue;
            }

            monster.Ai.ChangeState(MONSTER_STATE.Explore);
            Debug.Log("불러짐");
        }

        foreach (BaseMonster mosnter in deadMonsterSet)
        {
            _aliveMonsterSet.Remove(mosnter);
        }
    }

    #region Monster Spawn
    /// <summary>
    /// 스테이지에 따른 모든 몬스터 소환
    /// </summary>
    public void SpawnAllMonsters(int index, int posIndex = -1)
    {
        if (index < 0 || index >= _spawnDatas.Count)
        {
            Debug.LogError("[MonsterSpawner] 잘못된 스폰 데이터 인덱스를 입력했습니다.");
            return;
        }

        StartCoroutine(C_SpawnMonsters(index, posIndex));
    }

    /// <summary>
    /// 스폰 타임마다 몬스터를 소환하는 코루틴
    /// </summary>
    private IEnumerator C_SpawnMonsters(int spawnDataNum, int posIndex)
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

                SpawnMonster(spawnData.id, posIndex);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 해당하는 몬스터 ID를 가진 몬스터 소환
    /// </summary>
    private void SpawnMonster(int monsterId, int posIndex)
    {
        Vector3 pos = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
        if(posIndex != -1)
        {
            pos = _spawnPoints[posIndex].position;
        }

        GameObject obj = PoolManager.Instance.GetFromPool(monsterId, pos, _monsterParent);
        _aliveMonsterSet.Add(obj.GetComponent<BaseMonster>());
    }
    #endregion
}
