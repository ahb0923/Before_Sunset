using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerMaterialSlot : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    public int Index { get; private set; }
    
    public void InitIndex(int index)
    {
        Index = index;
    }

    public void SetSlot(ItemData data, int amount)
    {
        _iconImage.sprite = data.icon;
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
