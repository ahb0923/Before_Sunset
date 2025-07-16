using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class BuildingSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] public GameObject buildingPrefab;
    [SerializeField] public Image bGImage;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private TextMeshProUGUI _buildingName;
    
    private TowerDatabase _towerData;
    private SmelterDatabase _smelterData;
    private Color _originalColor;
    private Tween _tween;
    
    public int Index { get; private set; }
    
    private const string BUILDING_ICON = "BuildingIcon";
    private const string BUILDING_NAME = "BuildingName";
    
    private void Reset()
    {
        bGImage = GetComponent<Image>();
        _buildingIcon = Helper_Component.FindChildComponent<Image>(this.transform, BUILDING_ICON);
        _buildingName = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BUILDING_NAME);
    }
    
    public void InitIndex(int index)
    {
        Index = index;
        _originalColor = bGImage.color;
    }

    public void SetSlot(TowerDatabase data)
    {
        buildingPrefab = null;
        
        if (data != null)
        {
            _towerData = data;
            _smelterData = null;

            //var go = DataManager.Instance.TowerData.GetPrefabById(data.id);
            //buildingPrefab = go != null ? go.GetComponent<BaseTower>() : null;
            buildingPrefab = DataManager.Instance.TowerData.GetPrefabById(data.id);

            _buildingIcon.sprite = Helper_Component.FindChildComponent<SpriteRenderer>(buildingPrefab.transform,"Image").sprite;
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

            buildingPrefab = DataManager.Instance.SmelterData.GetPrefabById(_smelterData.id);

            _buildingIcon.sprite = Helper_Component.FindChildComponent<SpriteRenderer>(buildingPrefab.transform, "Image").sprite;
            _buildingIcon.preserveAspect = true;
            _buildingName.text = data.smelterName;

            this.gameObject.SetActive(true);
        }
    }
    /*
    public void SetSmelterSlot(SmelterDatabase smelterData)
    {
        buildingPrefab = null;
        
        if (smelterData != null)
        {
            _towerData = null;
            _smelterData = smelterData;
            
            // _buildingIcon = smelterData.icon;
            // _buildingIcon.preserveAspect = true;
            _buildingName.text = smelterData.smelterName;
            
            this.gameObject.SetActive(true);
        }
    }*/
    
    public void ClearSlot()
    {
        _towerData = null;
        _smelterData = null;
        _buildingIcon.sprite = null;
        _buildingName.text = "";
        gameObject.SetActive(false);
    }

    private string SetSmelt(SmelterDatabase data)
    {
        return $"제련 가능 광물\n" +
                    $"{DataManager.Instance.ItemData.GetId(data.smeltingIdList[0]).itemName}, " +
                    $"{DataManager.Instance.ItemData.GetId(data.smeltingIdList[1]).itemName}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        
        bGImage.color = _originalColor;
        _tween = bGImage.DOColor(Color.yellow, 0.2f);
        
        UIManager.Instance.CraftMaterialArea.Open();
        
        if (_towerData != null)
        {
            UIManager.Instance.CraftMaterialArea.SetMaterialSlot(_towerData);
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
            if (buildingPrefab != null)
            {
                if (!BuildManager.Instance.IsPlacing)
                {
                    BuildManager.Instance.StartPlacing(buildingPrefab);
                }
            }
        }
        // 우클릭시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (buildingPrefab != null)
            {
                if (BuildManager.Instance.IsPlacing)
                {
                    BuildManager.Instance.CancelPlacing();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        
        _tween = bGImage.DOColor(_originalColor, 0.2f);
        
        TooltipManager.Instance.HideTooltip();
        UIManager.Instance.CraftMaterialArea.Close();
    }
}