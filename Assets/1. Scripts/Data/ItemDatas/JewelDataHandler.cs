using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelDataHandler : BaseDataHandler<JewelDatabase>
{
    protected override string FileName => "JewelData_JSON.json";
    protected override int GetId(JewelDatabase data) => data.id;
    protected override string GetName(JewelDatabase data) => data.itemName;

    private Dictionary<int, Sprite> _jewelSprites = new();
    public Dictionary<int, Sprite> JewelSprites => _jewelSprites;

    public Sprite GetSpriteById(int id)
    {
        if (_jewelSprites.TryGetValue(id, out var sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"[JewelDataHandler] ID {id}에 해당하는 이미지가 존재하지 않습니다.");
        return null;
    }

    public void SettingImage()
    {
        foreach (var jewel in dataIdDictionary.Values)
        {
            Sprite jewelSprite = Resources.Load<Sprite>($"Items/Jewels/{jewel.itemName}");
            if (jewelSprite != null)
            {
                _jewelSprites.Add(jewel.id, jewelSprite);
            }
            else
            {
                Debug.LogWarning($"[Setting Sprite] 이미지 로드 실패: {jewel.id} / {jewel.itemName}");
            }
        }
        Debug.Log($"[Setting Sprite] 전체 보석 이미지 데이터 ({_jewelSprites.Count}개):");
    }
}
