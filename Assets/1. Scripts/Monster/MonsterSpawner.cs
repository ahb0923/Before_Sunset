using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private const int MONSTER_ID = 600;
    private const int STAGE_ID = 999;

    // 스폰 시작할 때 JSON에서 스폰 데이터를 받아옴
    private List<WaveData> _waveDatas;

    [Header("# Spawn Setting")]
    [SerializeField] private List<Transform> _spawnPoints;
    private int _spawnPointLimt => Mathf.Min((TimeManager.Instance.Stage - 1) / 3 + 1, _spawnPoints.Count - 1);
    [SerializeField] private Transform _monsterParent;
    private HashSet<BaseMonster> _aliveMonsterSet = new HashSet<BaseMonster>();
    public bool IsMonsterAlive => _aliveMonsterSet.Count > 0;

    [Header("# Test")]
    [SerializeField] List<WaveData> _testWaveDatas;

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
        // _waveData = DataManager.Instance.WaveData.GetById();

        if(_waveDatas == null)
        {
            if(_testWaveDatas == null)
            {
                Debug.LogWarning("[MonsterSpawner] 웨이브 데이터가 없습니다!");
                return;
            }

            _waveDatas = _testWaveDatas;
        }

        //List<WaveData> data = _waveDatas;
        //StartCoroutine(C_SpawnMonsters(data));

        StartCoroutine(C_SpawnMonsters(1));
    }
    /*
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
                    yield return null;
                }
            }

            yield return Helper_Coroutine.WaitSeconds(waveData.summonDelay);
            waveCount++;
        }

        TimeManager.Instance.OnSpawnOver();
    }*/

    /// <summary>
    /// C_SpanMonster 기준으로 만들었습니다. 제대로 동작할지는 잘 모르겠어요 테스트 필요합니다.<br/>
    /// 매개변수로는 stage 값을 받습니다. 1stage 라면 1 받으면 됩니다.
    /// </summary>
    /// <param name="stage"></param>
    /// <returns></returns>
    private IEnumerator C_SpawnMonsters(int stage)
    {
        var waveData = DataManager.Instance.WaveData;
        var monsterData = DataManager.Instance.MonsterData;

        // 1스테이지라면 매개변수값으로 1 입력!
        int waveCount = waveData.GetWaveCountByStageId(stage);

        for(int i=1; i<=waveCount; i++)
        {
            // 해당 스테이지의 웨이브 값을 매번 가져옴,
            // stage값은 매개변수로 받은 값으로  고정, wave는 i값으로 가변(최대 waveCount수까지 반복)
            // GetWaveByTupleKey 내부에서 각 매개변수에서 -1 씩해주기 때문에 실제 index값보다 +1된 값을 넣어야함
            var currentWaveData = waveData.GetWaveByTupleKey(stage, i);

            foreach (var pair in currentWaveData.waveInfo)
            {
                int monsterID = monsterData.GetByName(pair.Key).id;
                int spawnCount = pair.Value;
                
                for(int j=0; j<spawnCount; j++)
                {
                    SpawnMonster(monsterID, Random.Range(0, _spawnPointLimt));
                }
            }

            yield return Helper_Coroutine.WaitSeconds(currentWaveData.summonDelay);
        }
        TimeManager.Instance.OnSpawnOver();
    }



    /// <summary>
    /// 해당하는 몬스터 ID를 가진 몬스터 소환
    /// </summary>
    public void SpawnMonster(int monsterId, int posIndex)
    {
        float rand = Random.Range(-2f, 2f);
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
