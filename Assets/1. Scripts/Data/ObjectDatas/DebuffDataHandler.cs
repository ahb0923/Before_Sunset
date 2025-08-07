using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffDataHandler : BaseDataHandler<DebuffDatabase>
{
    protected override string FileName => "DebuffData_JSON.json";
    protected override int GetId(DebuffDatabase data) => data.id;
    protected override string GetName(DebuffDatabase data) => data.debuffName;

    private Dictionary<int, GameObject> _debuffPrefabs = new();
    public Dictionary<int, GameObject> DebuffPrefabs => _debuffPrefabs;

    public GameObject GetPrefabById(int id)
    {
        if (_debuffPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[DebuffDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }
    protected override void AfterLoaded()
    {
        SettingPrefab();
    }

    public void SettingPrefab()
    {
        foreach (var debuff in dataIdDictionary.Values)
        {
            GameObject debuffPrefab = Resources.Load<GameObject>($"Effects/Debuffs/{debuff.prefabName}");

            if (debuffPrefab != null)
            {
                _debuffPrefabs.Add(debuff.id, debuffPrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Prefab] 프리팹 로드 실패: {debuff.id} / {debuff.debuffName} / {debuff.prefabName}");
            }
        }
        Debug.Log($"[Setting Prefab] 전체 디버프 프리팹 데이터 ({_debuffPrefabs.Count}개):");
    }
}
