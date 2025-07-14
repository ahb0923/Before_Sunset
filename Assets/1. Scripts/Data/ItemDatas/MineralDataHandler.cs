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


    public Sprite GetSpriteById(int id)
    {
        if (_mineralSprites.TryGetValue(id, out var sprite))
        {
            return sprite;
        }

        Debug.LogWarning($"[MineralDataHandler] ID {id}에 해당하는 이미지가 존재하지 않습니다.");
        return null;
    }
    public void SettingImage()
    {
        foreach (var mineral in dataIdDictionary.Values)
        {
            Sprite mineralSprite;

            if (mineral.itemType == MINERAL_TYPE.Mineral)
            {
                mineralSprite = Resources.Load<Sprite>($"Items/Minerals/{mineral.itemName}");
            }
            else if (mineral.itemType == MINERAL_TYPE.Ingot)
            {
                mineralSprite = Resources.Load<Sprite>($"Items/Ingots/{mineral.itemName}");
            }
            else mineralSprite = null;


            if (mineralSprite != null)
            {
                _mineralSprites.Add(mineral.id, mineralSprite);
            }
            else
            {
                Debug.LogWarning($"[Setting Sprite] 이미지 로드 실패: {mineral.id} / {mineral.itemName}");
            }
        }
        Debug.Log($"[Setting Sprite] 전체 광물&주괴 이미지 데이터 ({_mineralSprites.Count}개):");
    }

}