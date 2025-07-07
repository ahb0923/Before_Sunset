using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMaterialSlot : MonoBehaviour
{
    public int Index { get; private set; }
    
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private TextMeshProUGUI _materialName;
    
    private const string MATERIAL_NAME = "BuildingMaterialName";
    
    private void Reset()
    {
        _iconImage = GetComponentInChildren<Image>();
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
        // _iconImage.sprite = data.icon;

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

    private int CountMaterial(string dataName, List<Item> items)
    {
        int count = 0;

        foreach (var item in items)
        {
            if (item.Data.itemName == dataName)
            {
                count += item.stack;
            }
        }
        
        return count;
    }
    
    public void ClearSlot()
    {
        _iconImage.sprite = null;
        _amountText.text = "";
        _materialName.text = "";
        gameObject.SetActive(false);
    }
}
