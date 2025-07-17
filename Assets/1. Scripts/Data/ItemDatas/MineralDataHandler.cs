using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class MineralDataHandler : BaseDataHandler<MineralDatabase>
{
    //protected override string DataUrl => "https://script.google.com/macros/s/AKfycbz0b4dwz-nu3icZa1vauBU0EWtUa8v259evQF4EJ_MWIkLiYZHvK0LbItdWmQ3gFdcb/exec";
    protected override string FileName => "MineralData_JSON.json";
    protected override int GetId(MineralDatabase data) => data.id;
    protected override string GetName(MineralDatabase data) => data.itemName;

    private Dictionary<int, Sprite> _mineralSprites = new();
    public Dictionary<int, Sprite> MineralSprites => _mineralSprites;

    private Dictionary<int, GameObject> _mineralPrefabs = new();
    public Dictionary<int, GameObject> MinerlaPrefabs => _mineralPrefabs;

    public Sprite GetSpriteById(int id)
    {
        if (_mineralSprites.TryGetValue(id, out var sprite))
        {
            return sprite;
        }

        Debug.LogWarning($"[MineralDataHandler] ID {id}에 해당하는 이미지가 존재하지 않습니다.");
        return null;
    }

    public GameObject GetPrefabById(int id)
    {
        if (_mineralPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[MineralDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }
    
    public void SettingPrefab()
    {
        foreach (var mineral in dataIdDictionary.Values)
        {
            Sprite mineralSprite;
            GameObject mineralPrefab;

            if (mineral.itemType == MINERAL_TYPE.Mineral)
            {
                mineralSprite = Resources.Load<Sprite>($"Items/Minerals/{mineral.itemName}");
                mineralPrefab = Resources.Load<GameObject>($"Items/Minerals/{mineral.prefabName}");
            }
            else if (mineral.itemType == MINERAL_TYPE.Ingot)
            {
                mineralSprite = Resources.Load<Sprite>($"Items/Ingots/{mineral.itemName}");
                mineralPrefab = Resources.Load<GameObject>($"Items/Ingots/{mineral.prefabName}");
            }
            else
            {
                mineralSprite = null;
                mineralPrefab = null;
            }

            if (mineralSprite != null && mineralPrefab != null)
            {
                _mineralSprites.Add(mineral.id, mineralSprite);
                _mineralPrefabs.Add(mineral.id, mineralPrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Sprite & Prefab] 이미지 & 프리팹 로드 실패: {mineral.id} / {mineral.itemName} / {mineral.prefabName}");
            }
        }
        Debug.Log($"[Setting Sprite & Prefab] 전체 광물&주괴 이미지 & 프리팹 데이터 ({_mineralSprites.Count}개 / {_mineralPrefabs.Count}개):");
    }

}