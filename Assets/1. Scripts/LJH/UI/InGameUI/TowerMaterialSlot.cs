using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerMaterialSlot : MonoBehaviour
{
    public int Index { get; private set; }
    
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    // private const string TOWER_MATERIAL_ICON = "TowerMaterialIcon";
    // private const string TOWER_MATERIAL_AMOUNT = "TowerMaterialAmount";

    private void Reset()
    {
        _iconImage = GetComponentInChildren<Image>();
        _amountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InitIndex(int index)
    {
        Index = index;
    }

    public void SetSlot(ItemData data, int amount)
    {
        
        
        /*_iconImage.sprite = data.icon;*/
        
        
        _amountText.text = amount.ToString();
        gameObject.SetActive(true);
    }
    
    public void ClearSlot()
    {
        _iconImage.sprite = null;
        _amountText.text = "";
        gameObject.SetActive(false);
    }
}
