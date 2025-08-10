using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BuildingSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] public GameObject buildingPrefab;
    [SerializeField] public Image bgImage;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private TextMeshProUGUI _buildingName;
    [SerializeField] private GameObject _disableImage;
    
    private TowerDatabase _towerData;
    private SmelterDatabase _smelterData;
    private int _currentBuildID;
    private Color _originalColor;
    private Tween _tween;
    public bool isDisabled = true;
    
    public int Index { get; private set; }
    
    private const string BUILDING_ICON = "BuildingIcon";
    private const string BUILDING_NAME = "BuildingName";
    private const string DISABLE_IMAGE = "DisableImage";
    
    private void Reset()
    {
        bgImage = GetComponent<Image>();
        _buildingIcon = Helper_Component.FindChildComponent<Image>(this.transform, BUILDING_ICON);
        _buildingName = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BUILDING_NAME);
        _disableImage = Helper_Component.FindChildGameObjectByName(this.gameObject, DISABLE_IMAGE);
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
            _currentBuildID = data.id;
            
            buildingPrefab = DataManager.Instance.TowerData.GetPrefabById(_currentBuildID);

            _buildingIcon.sprite = DataManager.Instance.TowerData.GetSpriteById(_currentBuildID);
         
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
            _currentBuildID = data.id;

            buildingPrefab = DataManager.Instance.SmelterData.GetPrefabById(_currentBuildID);
            
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

    public void CheckBuildable()
    {
        if (_towerData != null)
        {
            var buildRequirements = DataManager.Instance.TowerData.GetById(_currentBuildID).buildRequirements;
            int counter = 0;
            
            foreach (var item in buildRequirements)
            {
                if (InventoryManager.Instance.Inventory.GetItemCount(item.Key) < item.Value)
                {
                    counter++;
                    break;
                }
            }

            if (counter == 0)
                EnableSlot();
            else
                DisableSlot();
        }

        if (_smelterData != null)
        {
            var buildRequirements = DataManager.Instance.SmelterData.GetById(_currentBuildID).buildRequirements;
            int counter = 0;
            
            foreach (var item in buildRequirements)
            {
                if (InventoryManager.Instance.Inventory.GetItemCount(item.Key) < item.Value)
                {
                    counter++;
                    break;
                }
            }
            if (counter == 0)
                EnableSlot();
            else
                DisableSlot();
        }
    }

    private void DisableSlot()
    {
        isDisabled = true;
        _disableImage.SetActive(true);
    }

    private void EnableSlot()
    {
        isDisabled = false;
        _disableImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDisabled)
        {
            if (_tween != null)
            {
                _tween.Kill();
            }

            bgImage.color = _originalColor;
            _tween = bgImage.DOColor(Color.yellow, 0.2f);
        }
        
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
        bool isInBase = MapManager.Instance.Player.IsInBase;
        if (!isInBase)
        {
            ToastManager.Instance.ShowToast("광산에서는 설치할 수 없습니다.");
            return;
        }
        
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
                        var buildRequirements = DataManager.Instance.SmelterData.GetById(_currentBuildID).buildRequirements;
                        foreach (var item in buildRequirements)
                        {
                            if (InventoryManager.Instance.Inventory.GetItemCount(item.Key) < item.Value)
                            {
                                ToastManager.Instance.ShowToast("자원이 부족합니다!");
                                checkRequirements = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        var buildRequirements = DataManager.Instance.TowerData.GetById(_currentBuildID).buildRequirements;
                        foreach (var item in buildRequirements)
                        {
                            if (InventoryManager.Instance.Inventory.GetItemCount(item.Key) < item.Value)
                            {
                                ToastManager.Instance.ShowToast("자원이 부족합니다!");
                                checkRequirements = false;
                                break;
                            }
                        }
                    }
                }

                if (checkRequirements)
                {
                    BuildManager.Instance.StartPlacing(_currentBuildID);
                    UIManager.Instance.CraftArea.Close();
                }
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
        // if (!isDisabled)
        // {
        // }
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