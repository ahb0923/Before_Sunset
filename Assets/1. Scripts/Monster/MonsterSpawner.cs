using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private const int MONSTER_ID = 600;
    private const int STAGE_ID = 999;

    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    private int _spawnPointLimt => Mathf.Min((TimeManager.Instance.Stage - 1) / 3 + 1, _spawnPoints.Count - 1);
    [SerializeField] private Transform _monsterParent;
    private HashSet<BaseMonster> _aliveMonsterSet = new HashSet<BaseMonster>();
    public bool IsMonsterAlive => _aliveMonsterSet.Count > 0;

    private List<WaveData> _waveDatas;

    /// <summary>
    /// 몬스터 사망 시에 이 메서드를 호출하여 셋에서 제거
    /// </summary>
    public void RemoveDeadMonster(BaseMonster monster)
    {
        _aliveMonsterSet.Remove(monster);
    }

    /// <summary>
    /// 코어나 타워가 부서졌을 때에 모든 몬스터의 상태를 탐색 상태로 전환
    /// </summary>
    public void OnObstacleDestroyed()
    {
        foreach (BaseMonster monster in _aliveMonsterSet)
        {
            monster.Ai.ChangeState(MONSTER_STATE.Explore);
        }
    }

    /// <summary>
    /// 게임 오버 시에 모든 몬스터 못 움직이게 상태 전환
    /// </summary>
    public void OnGameOver()
    {
        foreach (BaseMonster monster in _aliveMonsterSet)
        {
            monster.Ai.ChangeState(MONSTER_STATE.Invalid);
        }
    }

    #region Monster Spawn
    /// <summary>
    /// 스테이지에 따른 모든 몬스터 소환
    /// </summary>
    public void SpawnAllMonsters()
    {
        // 모든 스테이지 데이터를 처음에 받아오면 좋겠는데 이건 생각해봐야 할 듯
        // _waveData = DataManager.Instance.WaveData.GetById();

        if(_waveDatas == null)
        {
            Debug.LogWarning("[MonsterSpawner] 웨이브 데이터가 없습니다!");
            return;
        }

        List<WaveData> data = _waveDatas;
        StartCoroutine(C_SpawnMonsters(data));
    }

    /// <summary>
    /// 스폰 타임마다 몬스터를 소환하는 코루틴
    /// </summary>
    private IEnumerator C_SpawnMonsters(List<WaveData> stageData)
    {
        int waveCount = 0;
        while(waveCount < stageData.Count)
        {
            while (TimeManager.Instance.IsGamePause)
                yield return null;

            WaveData waveData = stageData[waveCount];
            List<int> spawnCounts = waveData.spawnMonsterCount;

            for (int i = 0; i < spawnCounts.Count; i++) 
            {
                for (int j = 0; j < spawnCounts[i]; j++) 
                {
                    SpawnMonster(i + MONSTER_ID, Random.Range(0, _spawnPointLimt));
                }
            }

            yield return Helper_Coroutine.WaitSeconds(waveData.summonDelay);
            waveCount++;
        }

        TimeManager.Instance.OnSpawnOver();
    }

    /// <summary>
    /// 해당하는 몬스터 ID를 가진 몬스터 소환
    /// </summary>
    public void SpawnMonster(int monsterId, int posIndex)
    {
        Vector3 pos = _spawnPoints[posIndex].position;
        GameObject obj = PoolManager.Instance.GetFromPool(monsterId, pos, _monsterParent);
        _aliveMonsterSet.Add(obj.GetComponent<BaseMonster>());
    }
    #endregion
}
