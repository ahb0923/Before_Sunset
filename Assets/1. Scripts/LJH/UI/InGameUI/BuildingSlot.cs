using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BuildingSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] public BaseTower buildingPrefab;
    [SerializeField] private Image _bGImage;
    [SerializeField] private Image _buildingIcon;
    [SerializeField] private TextMeshProUGUI _buildingName;
    
    private TowerData _towerData;
    private SmelterData _smelterData;
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
        buildingPrefab = null;
        
        if (towerData != null)
        {
            _towerData = towerData;
            _smelterData = null;

            var go = DataManager.Instance.TowerData.GetTowerPrefabById(towerData.id);
            buildingPrefab = go != null ? go.GetComponent<BaseTower>() : null;

            _buildingIcon.sprite = Helper_Component.FindChildComponent<SpriteRenderer>(go.transform,"Image").sprite;
            _buildingName.text = towerData.towerName;
            
            this.gameObject.SetActive(true);
        }
    }

    public void SetSmelterSlot(SmelterData smelterData)
    {
        buildingPrefab = null;
        
        if (smelterData != null)
        {
            _towerData = null;
            _smelterData = smelterData;
            
            // _buildingIcon = smelterData.icon;
            _buildingName.text = smelterData.smelterName;
            
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tween != null)
        {
            _tween.Kill();
        }
        
        _bGImage.color = Color.white;
        _tween = _bGImage.DOColor(Color.yellow, 0.2f);
        
        UIManager.Instance.CraftMaterialArea.Open();
        
        if (_towerData != null)
        {
            UIManager.Instance.CraftMaterialArea.SetMaterialSlot(_towerData);
        }
        else if (_smelterData != null)
        {
            UIManager.Instance.CraftMaterialArea.SetMaterialSlot(_smelterData);
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
        
        _tween = _bGImage.DOColor(Color.white, 0.2f);
        
        UIManager.Instance.CraftMaterialArea.Close();
    }
}