using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TowerDataHandler : BaseDataHandler<TowerDatabase>
{
    //protected override string DataUrl => "https://script.google.com/macros/s/your-tower-sheet-id/exec";

    protected override string FileName => "TowerData_JSON.json";
    protected override int GetId(TowerDatabase data) => data.id;
    protected override string GetName(TowerDatabase data) => data.towerName;

    private Dictionary<int,GameObject> _towerPrefabs = new();
    public Dictionary<int, GameObject> TowerImages => _towerPrefabs;
    public GameObject GetPrefabById(int id)
    {
        if (_towerPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[TowerDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }
    /// <summary>
    /// 타워 이미지 데이터 초기화
    /// </summary>
    public void SettingPrefab()
    {
        foreach(var tower in dataIdDictionary.Values)
        {
            if(tower.buildType == TOWER_BUILD_TYPE.Base)
            {
                GameObject towerPrefab = Resources.Load<GameObject>($"Towers/{tower.prefabName}");
                if (towerPrefab != null)
                {
                    _towerPrefabs.Add(tower.id, towerPrefab);
                }
                else
                {
                    Debug.LogWarning($"[Setting Prefab] 프리팹 로드 실패: {tower.towerName} / {tower.prefabName}");
                }
            }
        }
        Debug.Log($"[Setting Prefab] 전체 타워 프리팹 데이터 ({_towerPrefabs.Count}개):");
    }

    [ContextMenu ("Debug all Log")]
    public override void DebugLogAll(Func<TowerDatabase, string> formatter = null)
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
                Debug.Log($"└ 타워 프리팹파일 : {tower.prefabName}");
            }
            else
            {
                Debug.Log($"└ 업그레이드 증가량 - HP: {tower.towerHp}, 공격력: {tower.damage}, 공속: {tower.aps}, 사거리: {tower.range}");
                Debug.Log($"└ 업그레이드 자원: {buildReqs}");
            }
        }
    }
}
