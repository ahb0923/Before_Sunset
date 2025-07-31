using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    private int _spawnPointLimt => Mathf.Min((TimeManager.Instance.Stage - 1) / 2, _spawnPoints.Count - 1);
    [SerializeField] private Transform _monsterParent;
    private HashSet<BaseMonster> _aliveMonsterSet = new HashSet<BaseMonster>();
    public bool IsMonsterAlive => _aliveMonsterSet.Count > 0;

    /// <summary>
    /// 몬스터 사망 시에 이 메서드를 호출하여 셋에서 제거
    /// </summary>
    public void RemoveDeadMonster(BaseMonster monster)
    {
        _aliveMonsterSet.Remove(monster);
        QuestManager.Instance?.AddQuestAmount(QUEST_TYPE.KillMonster, monster.GetId());
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

    /// <summary>
    /// 스테이지에 따른 모든 몬스터 소환
    /// </summary>
    public void SpawnAllMonsters()
    {
        UIManager.Instance.BattleUI.StartWarning();
        StartCoroutine(C_SpawnMonsters(TimeManager.Instance.Stage));
    }

    /// <summary>
    /// 웨이브 데이터를 받아와서 스테이지에 따른 웨이브 소환
    /// </summary>
    private IEnumerator C_SpawnMonsters(int stage)
    {
        if (GameManager.Instance.IsTutorial)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return Helper_Coroutine.WaitSeconds(0.5f);
                
                // 코어가 부서지면, 스폰 중지
                if (DefenseManager.Instance.Core.IsDead)
                {
                    yield break;
                }

                // 머프 10마리만 소환
                SpawnMonster(600, 0, true);
            }
        }
        else
        {
            int waveCount = DataManager.Instance.WaveData.GetWaveCountByStageId(stage);
            for (int i = 1; i <= waveCount; i++)
            {
                // GetWaveByTupleKey 내부에서 각 매개변수에서 -1 해줌
                WaveDatabase currentWaveData = DataManager.Instance.WaveData.GetWaveByTupleKey(stage, i);

                // 다음 웨이브 기다림
                yield return Helper_Coroutine.WaitSeconds(currentWaveData.summonDelay);

                // 몬스터 웨이브 소환
                foreach (var pair in currentWaveData.waveInfo)
                {
                    int monsterID = DataManager.Instance.MonsterData.GetByName(pair.Key).id;
                    int spawnCount = pair.Value;

                    // 열려있는 모든 방향에서 소환
                    for (int spawnIndex = 0;  spawnIndex <= _spawnPointLimt; spawnIndex++)
                    {
                        for (int j = 0; j < spawnCount; j++)
                        {
                            // 코어가 부서지면, 스폰 중지
                            if (DefenseManager.Instance.Core.IsDead)
                            {
                                yield break;
                            }

                            SpawnMonster(monsterID, spawnIndex, currentWaveData.isAttackCore);
                            yield return null;
                        }
                    }
                }
            }
        }

        // 모든 스폰이 끝났음을 알림
        TimeManager.Instance.OnSpawnOver();
    }

    /// <summary>
    /// 해당하는 몬스터 ID를 가진 몬스터 소환
    /// </summary>
    public void SpawnMonster(int monsterId, int posIndex, bool isAttackCore)
    {
        float rand = Random.Range(-1f, 1f);
        Vector3 randOffset;
        if (posIndex % 2 == 0)
            randOffset = new Vector3(rand, 0);
        else
            randOffset = new Vector3(0, rand);

        Vector3 pos = _spawnPoints[posIndex].position + randOffset;
        GameObject obj = PoolManager.Instance.GetFromPool(monsterId, pos, _monsterParent);
        BaseMonster monster = obj.GetComponent<BaseMonster>();
        monster.SetMonsterTargeting(isAttackCore);
        _aliveMonsterSet.Add(monster);
    }
}
