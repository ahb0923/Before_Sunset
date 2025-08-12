using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterDataHandler : BaseDataHandler<SmelterDatabase>
{
    protected override string FileName => "SmelterData_JSON.json";

    protected override int GetId(SmelterDatabase data) => data.id;

    protected override string GetName(SmelterDatabase data) => data.smelterName;


    private Dictionary<int, GameObject> _smelterPrefabs = new();
    public Dictionary<int, GameObject> SmelterPrefabs => _smelterPrefabs;
    public GameObject GetPrefabById(int id)
    {
        if (_smelterPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        //Debug.LogWarning($"[SmelterDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }

    protected override void AfterLoaded()
    {
        SettingPrefab();
    }

    public void SettingPrefab()
    {
        foreach (var smelter in dataIdDictionary.Values)
        {
            GameObject towerPrefab = Resources.Load<GameObject>($"Smelters/{smelter.prefabName}");
            if (towerPrefab != null)
            {
                _smelterPrefabs.Add(smelter.id, towerPrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Prefab] 프리팹 로드 실패: {smelter.smelterName} / {smelter.prefabName}");
            }
        }
        Debug.Log($"[Setting Prefab] 전체 제련소 프리팹 데이터 ({_smelterPrefabs.Count}개):");
    }

}
