using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JewelDataHandler : BaseDataHandler<JewelDatabase>
{
    protected override string FileName => "JewelData_JSON.json";
    protected override int GetId(JewelDatabase data) => data.id;
    protected override string GetName(JewelDatabase data) => data.itemName;

    private Dictionary<int, Sprite> _jewelSprites = new();
    public Dictionary<int, Sprite> JewelSprites => _jewelSprites;

    private Dictionary<int, GameObject> _jewelPrefabs = new();
    public Dictionary<int, GameObject> JewelPrefabs => _jewelPrefabs;

    public Sprite GetSpriteById(int id)
    {
        if (_jewelSprites.TryGetValue(id, out var sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"[JewelDataHandler] ID {id}에 해당하는 이미지가 존재하지 않습니다.");
        return null;
    }

    public GameObject GetPrefabById(int id)
    {
        if (_jewelPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[JewelDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }

    public void SettingPrefab()
    {
        foreach (var jewel in dataIdDictionary.Values)
        {
            Sprite jewelSprite = Resources.Load<Sprite>($"Items/Jewels/{jewel.itemName}");
            GameObject jewelPrefab = Resources.Load<GameObject>($"Items/Jewels/{jewel.prefabName}");

            if (jewelSprite != null && jewelPrefab != null)
            {
                _jewelSprites.Add(jewel.id, jewelSprite);
                _jewelPrefabs.Add(jewel.id, jewelPrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Sprite & Prefab] 이미지 & 프리팹 로드 실패: {jewel.id} / {jewel.itemName} / {jewel.prefabName}");
            }
        }
        Debug.Log($"[Setting Sprite & Prefab] 전체 보석 이미지 & 프리팹 데이터 ({_jewelSprites.Count}개 / {_jewelPrefabs.Count}개):");
    }
}
