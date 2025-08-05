using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private WarningArrow _arrowDisplay;

    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private int[] _stageToUnlockDoor = new int[3];
    [SerializeField] private Transform _monsterParent;
    [SerializeField] private float _warningTime = 1f;

    private HashSet<BaseMonster> _aliveMonsterSet = new HashSet<BaseMonster>();
    public bool IsMonsterAlive => _aliveMonsterSet.Count > 0;

    private void Awake()
    {
        _arrowDisplay = GetComponentInChildren<WarningArrow>();
    }

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

        if (GameManager.Instance.IsTutorial)
            StartCoroutine(C_SpawnTutorialMonsters());
        else
            StartCoroutine(C_SpawnMonsters(TimeManager.Instance.Day));
    }

    /// <summary>
    /// 웨이브 데이터를 받아와서 스테이지에 따른 웨이브 소환
    /// </summary>
    private IEnumerator C_SpawnMonsters(int day)
    {
        // 스테이지에 따라서 스폰 포인트가 늘어남
        int spawnPointNum = 1;
        foreach (int limit in _stageToUnlockDoor)
        {
            if (day >= limit)
            {
                spawnPointNum++;
            }
        }

        // 스폰 포인트가 1곳이면, 모든 웨이브 한 방향에서만 나옴
        List<int> spawnPointList = null;
        if (spawnPointNum == 1)
            spawnPointList = GetSpawnPointList(spawnPointNum);

        int waveCount = DataManager.Instance.WaveData.GetWaveCountByStageId(day);
        for (int i = 1; i <= waveCount; i++)
        {
            // GetWaveByTupleKey 내부에서 각 매개변수에서 -1 해줌
            WaveDatabase currentWaveData = DataManager.Instance.WaveData.GetWaveByTupleKey(day, i);

            // 다음 웨이브 기다림
            yield return Helper_Coroutine.WaitSeconds(currentWaveData.summonDelay);

            // 스폰 포인트가 여러 곳이면, 웨이브마다 무작위 방향에서 나옴
            if (spawnPointNum != 1)
                spawnPointList = GetSpawnPointList(spawnPointNum);

            // 몬스터 웨이브 방향 이펙트 표시
            yield return C_DisplaySpawnDirection(spawnPointList);

            // 몬스터 웨이브 소환
            foreach (var pair in currentWaveData.waveInfo)
            {
                int monsterID = DataManager.Instance.MonsterData.GetByName(pair.Key).id;
                int spawnCount = pair.Value;

                for (int j = 0; j < spawnCount; j++)
                {
                    for (int k = 0; k < spawnPointList.Count; k++)
                    {
                        // 코어가 부서지면, 스폰 중지
                        if (DefenseManager.Instance.Core.IsDead)
                        {
                            yield break;
                        }

                        SpawnMonster(monsterID, spawnPointList[k]);
                        yield return null;
                    }
                }
            }
        }

        // 모든 스폰이 끝났음을 알림
        TimeManager.Instance.OnSpawnOver();
    }

    /// <summary>
    /// 튜토리얼 몬스터 소환
    /// </summary>
    private IEnumerator C_SpawnTutorialMonsters()
    {
        yield return C_DisplaySpawnDirection(new List<int> {0});

        for (int i = 0; i < 3; i++)
        {
            yield return Helper_Coroutine.WaitSeconds(0.5f);

            // 코어가 부서지면, 스폰 중지
            if (DefenseManager.Instance.Core.IsDead)
            {
                yield break;
            }

            // 머프 3마리만 소환
            SpawnMonster(600, 0);
        }
    }

    /// <summary>
    /// 몬스터 스폰 포인트 리스트 반환
    /// </summary>
    private List<int> GetSpawnPointList(int spawnPointNum)
    {
        List<int> spawnPointList = new List<int>() {0, 1, 2, 3};
        for (int i = 0; i < 4 - spawnPointNum; i++)
        {
            spawnPointList.RemoveAt(Random.Range(0, spawnPointList.Count));
        }

        return spawnPointList;
    }

    /// <summary>
    /// 방향 이펙트 표시
    /// </summary>
    private IEnumerator C_DisplaySpawnDirection(List<int> spawnPoints)
    {
        foreach(int spawnPoint in spawnPoints)
        {
            _arrowDisplay.OnWarning((WARNING_DIRECTION)spawnPoint);
        }

        yield return Helper_Coroutine.WaitSeconds(_warningTime);

        _arrowDisplay.OffAllWarning();
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
        BaseMonster monster = obj.GetComponent<BaseMonster>();
        _aliveMonsterSet.Add(monster);
    }
}
