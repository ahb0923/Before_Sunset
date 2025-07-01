using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TowerDataHandler : BaseDataHandler<TowerData>
{
    //protected override string DataUrl => "https://script.google.com/macros/s/your-tower-sheet-id/exec";

    protected override string FileName => "TowerData_JSON.json";
    protected override int GetId(TowerData data) => data.id;
    protected override string GetName(TowerData data) => data.towerName;



    [ContextMenu ("Debug all Log")]
    public override void DebugLogAll(Func<TowerData, string> formatter = null)
    {
        if (dataIdDictionary.Count == 0)
        {
            Debug.LogWarning("[TowerDataHandler] 로드된 타워 데이터가 없습니다.");
            return;
        }

        Debug.Log($"[TowerDataHandler] 전체 타워 데이터 ({dataIdDictionary.Count}개):");

        foreach (var tower in dataIdDictionary.Values)
        {
            if (formatter != null)
            {
                Debug.Log(formatter(tower));
                continue;
            }

            string buildReqs = string.Join(", ", tower.buildRequirements.Select(kv => $"ID {kv.Key}: {kv.Value}"));

            Debug.Log($"[타워] ID: {tower.id}, 이름: {tower.towerName}, 티어: {tower.level}");
            Debug.Log($"└ 설명: {tower.flavorText}");

            if (tower.buildType == TOWER_BUILD_TYPE.Base)
            {
                Debug.Log($"└ 스탯 - HP: {tower.towerHp}, 공격력: {tower.damage}, 공속: {tower.aps}, 사거리: {tower.range}");
                Debug.Log($"└ 제작 자원: {buildReqs}");
            }
            else
            {
                Debug.Log($"└ 업그레이드 증가량 - HP: {tower.towerHp}, 공격력: {tower.damage}, 공속: {tower.aps}, 사거리: {tower.range}");
                Debug.Log($"└ 업그레이드 자원: {buildReqs}");
            }
        }
    }
}
