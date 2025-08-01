using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDataHandler : BaseDataHandler<EquipmentDatabase>
{
    protected override string FileName => "EquipmentData_JSON.json";
    protected override int GetId(EquipmentDatabase data) => data.id;
    protected override string GetName(EquipmentDatabase data) => data.itemName;

    private Dictionary<int, Sprite> _spriteDictionary = new();
    public Dictionary<int, Sprite> SpriteDictionary => _spriteDictionary;

    protected override void AfterLoaded()
    {
        SettingPrefab();
    }

    public void SettingPrefab()
    {
        foreach (var equipments in dataIdDictionary.Values)
        {
            Sprite equipmentsSprite = Resources.Load<Sprite>($"Items/Equipments/{equipments.spriteName}");

            if (equipmentsSprite != null)
            {
                _spriteDictionary.Add(equipments.id, equipmentsSprite);
            }
            else
            {
                Debug.LogWarning($"[Setting Prefab] 이미지 로드 실패: {equipments.id} / {equipments.itemName} / {equipments.spriteName}");
            }
        }
        Debug.Log($"[Setting Prefab] 전체 장비(곡괭이) 이미지 데이터 ({_spriteDictionary.Count}개):");
    }

    public Sprite GetSpriteById(int id)
    {
        SpriteDictionary.TryGetValue(id, out Sprite sprite);
        return sprite;
    }
}
