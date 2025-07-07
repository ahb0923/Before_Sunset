using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BuildingSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private Image _bGImage;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private TextMeshProUGUI _buildingName;
    
    private Tween _tween;
    
    public int Index { get; private set; }
    
    private const string BUILDING_ICON = "BuildingIcon";
    private const string BUILDING_NAME = "BuildingName";
    
    private void Reset()
    {
        _bGImage = GetComponent<Image>();
        _buildingIcon = Helper_Component.FindChildComponent<Image>(this.transform, BUILDING_ICON);
        _buildingName = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BUILDING_NAME);
    }
    
    public void InitIndex(int index)
    {
        Index = index;
    }

    public void SetTowerSlot(TowerData towerData)
    {
        if (towerData != null)
        {
            // _buildingIcon = towerData.icon;
            _buildingName.text = towerData.towerName;
            
            this.gameObject.SetActive(true);
        }
        else
        {
            _buildingIcon = null;
            _buildingName.text = "";
            
            this.gameObject.SetActive(false);
        }
    }

    // public void SetSmelterSlot(SmelterData smelterData)
    // {
    //     if (smelterData != null)
    //     {
    //         _buildingIcon = smelterData.icon;
    //         _buildingName.text = smelterData.smelterName;
    //         
    //         this.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         _buildingIcon = null;
    //         _buildingName.text = "";
    //         
    //         this.gameObject.SetActive(false);
    //     }
    // }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        
        if (_tween != null)
        {
            _tween.Kill();
        }
        
        _bGImage.color = Color.white;

        _tween = _bGImage.DOColor(Color.yellow, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }

        _tween = _bGImage.DOColor(Color.white, 0.2f);
    }
}