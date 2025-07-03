using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnData
{
    public int id;
    public int spawnCount;
}

[System.Serializable]
public class StageData
{
    public int stage;
    public List<SpawnData> spawnDatas;
}

public class MonsterSpawner : MonoBehaviour
{
    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    private int _spawnPointLimt => Mathf.Min((TimeManager.Instance.Stage - 1) / 3 + 1, _spawnPoints.Count - 1);
    [SerializeField] private float _spawnTime;
    [SerializeField] private Transform _monsterParent;
    [SerializeField] private List<StageData> _stageData;
    private Dictionary<int, StageData> _stageDict;

    private HashSet<BaseMonster> _aliveMonsterSet = new HashSet<BaseMonster>();
    public bool IsMonsterAlive => _aliveMonsterSet.Count > 0;

    private void Awake()
    {
        _stageDict = new Dictionary<int, StageData>();

        foreach (StageData data in _stageData)
        {
            _stageDict[data.stage] = data;
        }
    }

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
        StageData data = _stageDict[TimeManager.Instance.Stage];
        StartCoroutine(C_SpawnMonsters(data));
    }

    /// <summary>
    /// 스폰 타임마다 몬스터를 소환하는 코루틴
    /// </summary>
    private IEnumerator C_SpawnMonsters(StageData stageData)
    {
        int monsterCount = 0;
        while(monsterCount < stageData.spawnDatas.Count)
        {
            SpawnData spawnData = stageData.spawnDatas[monsterCount];
            for (int i = 0; i < spawnData.spawnCount; i++)
            {
                // 일시 정지 시에는 스폰 중지
                while (TimeManager.Instance.IsGamePause)
                    yield return null;
                
                SpawnMonster(spawnData.id, Random.Range(0, _spawnPointLimt));
                yield return Helper_Coroutine.WaitSeconds(_spawnTime);
            }

            monsterCount++;
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
