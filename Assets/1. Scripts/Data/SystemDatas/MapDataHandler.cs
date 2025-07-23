using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataHandler : BaseDataHandler<MapDatabase>
{
    protected override string FileName => "MapData_JSON.json";
    protected override int GetId(MapDatabase data) => data.id;
    protected override string GetName(MapDatabase data) => data.mapName;
    private Dictionary<int, GameObject> _mapPrefabs = new();
    public Dictionary<int, GameObject> MapPrefabs => _mapPrefabs;


    public GameObject GetPrefabById(int id)
    {
        if (_mapPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[MapDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }

    protected override void AfterLoaded()
    {
        SettingPrefab();
    }
    /// <summary>
    /// 타워 프리팹 및 이미지 데이터 초기화
    /// </summary>
    public void SettingPrefab()
    {
        foreach (var map in dataIdDictionary.Values)
        {
            GameObject mapPrefab = Resources.Load<GameObject>($"Maps/{map.prefabName}");
            if (mapPrefab != null)
            {
                _mapPrefabs.Add(map.id, mapPrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Prefab] 프리팹 로드 실패: [{map.id}] : {map.mapName} / {map.prefabName}");
            }
        }
        Debug.Log($"[Setting Prefab] 전체 맵 프리팹 데이터 ({_mapPrefabs.Count}개):");
    }
}
