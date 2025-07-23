using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDataHandler : BaseDataHandler<OreDatabase>
{
    protected override string FileName => "OreData_JSON.json";
    protected override int GetId(OreDatabase data) => data.id;
    protected override string GetName(OreDatabase data) => data.itemName;

    private Dictionary<int, GameObject> _orePrefabs = new();
    public Dictionary<int, GameObject> OrePrefabs => _orePrefabs;

    public GameObject GetPrefabById(int id)
    {
        if (_orePrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[OreDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }
    protected override void AfterLoaded()
    {
        SettingPrefab();
    }

    public void SettingPrefab()
    {
        foreach (var ore in dataIdDictionary.Values)
        {
            GameObject orePrefab = Resources.Load<GameObject>($"Ores/{ore.prefabName}");

            if (orePrefab != null)
            {
                _orePrefabs.Add(ore.id, orePrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Prefab] 프리팹 로드 실패: {ore.id} / {ore.itemName} / {ore.prefabName}");
            }
        }
        Debug.Log($"[Setting Prefab] 전체 광석 프리팹 데이터 ({_orePrefabs.Count}개):");
    }
}
