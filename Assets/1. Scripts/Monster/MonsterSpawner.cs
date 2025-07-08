using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    private int _spawnPointLimt => Mathf.Min((TimeManager.Instance.Stage - 1) / 3 + 1, _spawnPoints.Count - 1);
    [SerializeField] private Transform _monsterParent;
    private HashSet<BaseMonster> _aliveMonsterSet = new HashSet<BaseMonster>();
    public bool IsMonsterAlive => _aliveMonsterSet.Count > 0;

    /// <summary>
    /// 몬스터 사망 시에 이 메서드를 호출하여 셋에서 제거
    /// </summary>
    public void RemoveDeadMonster(BaseMonster monster)
    {
        _aliveMonsterSet.Remove(monster);
    }

    /// <summary>
    /// 맵 장애물에 변경이 있을 때, 모든 몬스터의 상태를 탐색 상태로 전환
    /// </summary>
    public void OnObstacleChanged()
    {
        foreach (BaseMonster monster in _aliveMonsterSet)
        {
            monster.Ai.ChangeState(MONSTER_STATE.Explore);
        }
    }

    #region Monster Spawn
    /// <summary>
    /// 스테이지에 따른 모든 몬스터 소환
    /// </summary>
    public void SpawnAllMonsters()
    {
        StartCoroutine(C_SpawnMonsters(TimeManager.Instance.Stage));
    }

    /// <summary>
    /// 웨이브 데이터를 받아와서 스테이지에 따른 웨이브 소환
    /// </summary>
    private IEnumerator C_SpawnMonsters(int stage)
    {
        var waveData = DataManager.Instance.WaveData;
        var monsterData = DataManager.Instance.MonsterData;

        int waveCount = waveData.GetWaveCountByStageId(stage);

        for (int i = 1; i <= waveCount; i++)
        {
            // GetWaveByTupleKey 내부에서 각 매개변수에서 -1 해줌
            var currentWaveData = waveData.GetWaveByTupleKey(stage, i);

            // 다음 웨이브 기다림
            yield return Helper_Coroutine.C_WaitIfNotPaused(currentWaveData.summonDelay, () => TimeManager.Instance.IsGamePause);

            // 몬스터 뭉탱이 소환
            foreach (var pair in currentWaveData.waveInfo)
            {
                int monsterID = monsterData.GetByName(pair.Key).id;
                int spawnCount = pair.Value;

                for (int j = 0; j < spawnCount; j++)
                {
                    // 게임 오버 시에 코루틴 탈출 필요

                    SpawnMonster(monsterID, Random.Range(0, _spawnPointLimt));
                    yield return null;
                }
            }
        }

        // 모든 스폰이 끝났음을 알림
        TimeManager.Instance.OnSpawnOver();
    }

    /// <summary>
    /// 해당하는 몬스터 ID를 가진 몬스터 소환
    /// </summary>
    public void SpawnMonster(int monsterId, int posIndex)
    {
        float rand = Random.Range(-1f, 1f);
        Vector3 randOffset;
        if (posIndex % 2 == 0)
            randOffset = new Vector3(rand, 0);
        else
            randOffset = new Vector3(0, rand);

        Vector3 pos = _spawnPoints[posIndex].position + randOffset;
        GameObject obj = PoolManager.Instance.GetFromPool(monsterId, pos, _monsterParent);
        _aliveMonsterSet.Add(obj.GetComponent<BaseMonster>());
    }
    #endregion
}
