using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CraftArea : MonoBehaviour
{
    [SerializeField] private Button _buildButton;
    [SerializeField] private Button _towerButton;
    [SerializeField] private Button _smelterButton;
    [SerializeField] private GameObject _buildingSlotArea;
    [SerializeField] private GameObject _buildingSlotPrefab;
    [SerializeField] private List<BuildingSlot> _buildSlots = new List<BuildingSlot>();
    [SerializeField] private List<TowerDatabase> _baseTowerData = new List<TowerDatabase>();
    [SerializeField] private List<SmelterDatabase> _smelterData;
    
    private const string BUILD_BUTTON = "BuildButton";
    private const string TOWER_BUTTON = "TowerButton";
    private const string SMELTER_BUTTON = "SmelterButton";
    private const string BUILD_SLOT = "Slots/BuildingSlot";
    private const string BUILD_SLOT_AREA = "BuildingSlotArea";

    private void Reset()
    {
        _buildButton = Helper_Component.FindChildComponent<Button>(this.transform.parent, BUILD_BUTTON);
        _towerButton = Helper_Component.FindChildComponent<Button>(this.transform, TOWER_BUTTON);
        _smelterButton = Helper_Component.FindChildComponent<Button>(this.transform, SMELTER_BUTTON);
        _buildingSlotArea = Helper_Component.FindChildGameObjectByName(this.gameObject, BUILD_SLOT_AREA);
        _buildingSlotPrefab = Resources.Load<GameObject>(BUILD_SLOT);
    }

    public void Awake()
    {
        _buildButton.onClick.AddListener(Toggle);
        _towerButton.onClick.AddListener(TowerButton);
        _smelterButton.onClick.AddListener(SmelterButton);
        
        GetBaseTowerList();
        GetSmelterList();
        InitSlots();
        TowerButton();
    }

    private void TowerButton()
    {
        _towerButton.interactable = false;
        _smelterButton.interactable = true;

        foreach (var slot in _buildSlots)
        {
            slot.ClearSlot();
        }

        for (int i = 0; i < _buildSlots.Count; i++)
        {
            TowerDatabase data = i < _baseTowerData.Count ? _baseTowerData[i] : null;
            _buildSlots[i].SetTowerSlot(data);
        }
    }
    
    private void SmelterButton()
    {
        _smelterButton.interactable = false;
        _towerButton.interactable = true;
        
        foreach (var slot in _buildSlots)
        {
            slot.ClearSlot();
        }
    
        for (int i = 0; i < _buildSlots.Count; i++)
        {
            SmelterDatabase data = i < _smelterData.Count ? _smelterData[i] : null;
            _buildSlots[i].SetSmelterSlot(data);
        }
    }

    public void GetBaseTowerList()
    {
        var towerDatas = DataManager.Instance.TowerData.GetAllItems();

        foreach (var towerData in towerDatas)
        {
            if (towerData.buildType == TOWER_BUILD_TYPE.Base)
            {
                _baseTowerData.Add(towerData);
            }
        }
    }
    
    public void GetSmelterList()
    {
        _smelterData = DataManager.Instance.SmelterData.GetAllItems();
    }

    private void InitSlots()
    {
        int requiredSlot = Math.Max(_baseTowerData.Count, _smelterData.Count);
    
        for (int i = 0; i < requiredSlot; i++)
        {
            var slot = Instantiate(_buildingSlotPrefab, _buildingSlotArea.transform);
            var buildingSlot = slot.GetComponent<BuildingSlot>();
            _buildSlots.Add(buildingSlot);
            buildingSlot.InitIndex(i);
        }
    }
    
    public void ToggleCraftArea()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Toggle();
        }
    }

    public void Toggle()
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
            UIManager.Instance.CraftMaterialArea.gameObject.SetActive(false);
            
            ResetUI();
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }

    private void ResetUI()
    {
        if (_buildSlots.Count != 0)
        {
            foreach (var slot in _buildSlots)
            {
                slot.bGImage.color = Color.white;
            }

            if (TooltipManager.Instance.isOpen)
                TooltipManager.Instance.HideTooltip();
        }
    }
}
