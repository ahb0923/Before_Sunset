using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveDataHandler : BaseDataHandler<WaveDatabase>
{
    private const int STAGE_ID = 1000;
    private const int GROUP_SIZE = 10;

    private Dictionary<(int stageIndex, int waveIndex), WaveDatabase> _waveDatas = new();


    protected override string FileName => "WaveData_JSON.json";
    protected override int GetId(WaveDatabase data) => data.stageID;
    protected override string GetName(WaveDatabase data) => null;

    /// <summary>
    /// 데이터 베이스의 stageId를 이용하여 데이터를 찾음
    /// </summary>
    /// <param name="stageId"></param>
    /// <returns></returns>
    public WaveDatabase GetWaveByStageId(int stageId)
    {
        var key = GetTupleKey(stageId);
        return _waveDatas.TryGetValue(key, out var data) ? data : null;

    }

    /// <summary>
    /// 웨이브 아이디 넣어서 웨이브 갯수 받아오기
    /// </summary>
    /// <param name="stageId"></param>
    /// <returns></returns>
    public int GetWaveCountByStageId(int stageIndex)
    {
        int count = 0;

        return count = _waveDatas.Count(pair => pair.Key.stageIndex == (stageIndex - 1));
    }

    /// <summary>
    /// (스테이지, 웨이브) 형식으로 데이터를 찾음<br/>
    /// 1스테이지 1웨이브 이면 1,1 입력하면 됨
    /// </summary>
    /// <returns></returns>
    public WaveDatabase GetWaveByTupleKey(int stageIndex, int waveIndex)
    {
        return _waveDatas.TryGetValue((stageIndex-1, waveIndex-1), out var data) ? data : null;
    }

    protected override void AfterLoaded()
    {
        _waveDatas.Clear();

        foreach(var wave in dataIdDictionary.Values)
        {
            var key = GetTupleKey(wave.stageID);
            _waveDatas[key] = wave;
        }
    }


    /// <summary>
    /// 튜플 키 생성기
    /// </summary>
    /// <param name="stageId"></param>
    /// <returns></returns>
    private (int stageIndex, int waveIndex) GetTupleKey(int stageId)
    {
        int stageIndex = (stageId - STAGE_ID) / GROUP_SIZE;
        int waveIndex = (stageId - STAGE_ID) % GROUP_SIZE;
        return (stageIndex, waveIndex);
    }

    [ContextMenu("Debug all Log")]
    public override void DebugLogAll(Func<WaveDatabase, string> formatter = null)
    {
        if (dataIdDictionary.Count == 0)
        {
            Debug.LogWarning("[WaveDataHandler] 로드된 타워 데이터가 없습니다.");
            return;
        }

        Debug.Log($"[WaveDataHandler] 전체 웨이브 데이터 ({dataIdDictionary.Count}개):");

        foreach (var kvp in _waveDatas.OrderBy(k => k.Key.stageIndex).ThenBy(k => k.Key.waveIndex))
        {
            var (stageIndex, waveIndex) = kvp.Key;
            var wave = kvp.Value;

            string monsterInfo = wave.waveInfo != null && wave.waveInfo.Count > 0
                ? string.Join(", ", wave.waveInfo.Select(kv => $"{kv.Key}:{kv.Value}"))
                : "없음";

            Debug.Log($"▶ [스테이지 {stageIndex + 1}, 웨이브 {waveIndex + 1}] (stageID: {wave.stageID})");
            Debug.Log($"   소환 딜레이: {wave.summonDelay}, 코어 공격 여부: {wave.isAttackCore}");
            Debug.Log($"   몬스터 구성: {monsterInfo}");
        }
    }

}
