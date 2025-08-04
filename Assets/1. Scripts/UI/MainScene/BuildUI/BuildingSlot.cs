using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Serialization;

public class BuildingSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] public GameObject buildingPrefab;
    [SerializeField] public Image bgImage;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private TextMeshProUGUI _buildingName;
    
    private TowerDatabase _towerData;
    private SmelterDatabase _smelterData;
    private int currentBuildID;
    private Color _originalColor;
    private Tween _tween;
    
    public int Index { get; private set; }
    
    private const string BUILDING_ICON = "BuildingIcon";
    private const string BUILDING_NAME = "BuildingName";
    
    private void Reset()
    {
        bgImage = GetComponent<Image>();
        _buildingIcon = Helper_Component.FindChildComponent<Image>(this.transform, BUILDING_ICON);
        _buildingName = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BUILDING_NAME);
    }
    
    public void InitIndex(int index)
    {
        Index = index;
        _originalColor = bgImage.color;
    }

    public void SetSlot(TowerDatabase data)
    {
        buildingPrefab = null;
        
        if (data != null)
        {
            _towerData = data;
            _smelterData = null;
            currentBuildID = data.id;
            
            buildingPrefab = DataManager.Instance.TowerData.GetPrefabById(currentBuildID);

            _buildingIcon.sprite = DataManager.Instance.TowerData.GetSpriteById(currentBuildID);
            _buildingIcon.preserveAspect = true;
            _buildingName.text = data.towerName;
            
            this.gameObject.SetActive(true);
        }
    }
    public void SetSlot(SmelterDatabase data)
    {
        buildingPrefab = null;

        if (data != null)
        {
            _towerData = null;
            _smelterData = data;
            currentBuildID = data.id;

            buildingPrefab = DataManager.Instance.SmelterData.GetPrefabById(currentBuildID);
            
            _buildingIcon.sprite = Helper_Component.GetComponentInChildren<SpriteRenderer>(buildingPrefab).sprite;
            _buildingIcon.preserveAspect = true;
            _buildingName.text = data.smelterName;

            this.gameObject.SetActive(true);
        }
    }
    
    public void ClearSlot()
    {
        _towerData = null;
        _smelterData = null;
        _buildingIcon.sprite = null;
        _buildingName.text = "";
        gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        bgImage.color = _originalColor;
    }

    private string SetSmelt(SmelterDatabase data)
    {
        return $"제련 가능 광물\n" +
                    $"{DataManager.Instance.ItemData.GetById(data.smeltingIdList[0]).itemName}, " +
                    $"{DataManager.Instance.ItemData.GetById(data.smeltingIdList[1]).itemName}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        
        bgImage.color = _originalColor;
        _tween = bgImage.DOColor(Color.yellow, 0.2f);
        
        UIManager.Instance.CraftMaterialArea.Open();
        
        if (_towerData != null)
        {
            UIManager.Instance.CraftMaterialArea.SetMaterialSlot(_towerData);
            TooltipManager.Instance.ShowTooltip(_towerData.towerName, _towerData.flavorText);
        }
        else if (_smelterData != null)
        {
            UIManager.Instance.CraftMaterialArea.SetMaterialSlot(_smelterData);
            TooltipManager.Instance.ShowTooltip(_smelterData.smelterName, SetSmelt(_smelterData));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭시
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!BuildManager.Instance.IsPlacing)
            {
                bool checkRequirements = true;
                if (!GameManager.Instance.GOD_MODE)
                {

                    if (_towerData == null)
                    {
                        var buildRequirements = DataManager.Instance.SmelterData.GetById(currentBuildID).buildRequirements;
                        foreach (var item in buildRequirements)
                        {
                            if (InventoryManager.Instance.Inventory.GetItemCount(item.Key) < item.Value)
                            {
                                ToastManager.Instance.ShowToast("자원이 부족합니다!");
                                checkRequirements = false;
                            }
                        }
                    }
                    else
                    {
                        var buildRequirements = DataManager.Instance.TowerData.GetById(currentBuildID).buildRequirements;
                        foreach (var item in buildRequirements)
                        {
                            if (InventoryManager.Instance.Inventory.GetItemCount(item.Key) < item.Value)
                            {
                                ToastManager.Instance.ShowToast("자원이 부족합니다!");
                                checkRequirements = false;
                            }
                        }
                    }
                }
                if(checkRequirements)
                    BuildManager.Instance.StartPlacing(currentBuildID);
                UIManager.Instance.CraftArea.Close();
            }
        }
        // 우클릭시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(BuildManager.Instance.IsPlacing)
            {
                BuildManager.Instance.CancelPlacing();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
        
        _tween = bgImage.DOColor(_originalColor, 0.2f);
        
        TooltipManager.Instance.HideTooltip();
        UIManager.Instance.CraftMaterialArea.Close();
    }

    private void OnDisable()
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }
        bgImage.color = _originalColor;
    }
}