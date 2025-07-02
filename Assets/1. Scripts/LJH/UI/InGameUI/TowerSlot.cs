using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private Image _towerIcon;
    [SerializeField] private TextMeshProUGUI _towerName;
    
    private const string TOWER_ICON = "TowerIcon";
    private const string TOWER_NAME = "TowerName";

    public int Index { get; private set; }
    
    private void Reset()
    {
        _towerIcon = UtilityLJH.FindChildComponent<Image>(this.transform, TOWER_ICON);
        _towerName = UtilityLJH.FindChildComponent<TextMeshProUGUI>(this.transform, TOWER_NAME);
    }
    
    public void InitIndex(int index)
    {
        Index = index;
    }

    public void SetSlot()
    {
        var index = 400 + (Index * 10);
        TowerData data = DataManager.Instance.TowerData.GetById(index);
        
        
        /*_iconImage.sprite = data.icon;*/


        //_towerName.text = data.towerName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}