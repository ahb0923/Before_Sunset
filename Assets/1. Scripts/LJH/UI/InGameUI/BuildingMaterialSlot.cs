using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuildingMaterialSlot : MonoBehaviour
{
    public int Index { get; private set; }
    
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private TextMeshProUGUI _materialName;
    
    private const string MATERIAL_NAME = "BuildingMaterialName";
    
    private void Reset()
    {
        _itemImage = GetComponentInChildren<Image>();
        _amountText = GetComponentInChildren<TextMeshProUGUI>();
        _materialName = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, MATERIAL_NAME);
    }

    public void InitIndex(int index)
    {
        Index = index;
    }
    
    public void SetSlot(string dataName, int requiredAmount, List<Item> items)
    {
        var data = DataManager.Instance.ItemData.GetName(dataName);
        var amount = CountMaterial(dataName, items);
        
        _materialName.text = dataName;
        _amountText.text = $"{amount}/{requiredAmount}";
        SetImage(data);

        if (amount < requiredAmount)
        {
            _amountText.color = Color.red;
            
        }
        else
        {
            _amountText.color = Color.black;
        }
        
        gameObject.SetActive(true);
    }
    
    private void SetImage(ItemDatabase data)
    {
        if (data.id >= 100 && data.id < 200)
            _itemImage.sprite = DataManager.Instance.MineralData.GetSpriteById(data.id);
        else if (data.id >= 200 && data.id < 300)
            _itemImage.sprite = DataManager.Instance.JewelData.GetSpriteById(data.id);
    }

    private int CountMaterial(string dataName, List<Item> items)
    {
        int count = 0;

        foreach (var item in items)
        {
            if (item == null)
            {
                continue;
            }
            
            if (item.Data.itemName == dataName)
            {
                count += item.stack;
            }
        }
        
        return count;
    }
    
    public void ClearSlot()
    {
        _itemImage.sprite = null;
        _amountText.text = "";
        _materialName.text = "";
        gameObject.SetActive(false);
    }
}
