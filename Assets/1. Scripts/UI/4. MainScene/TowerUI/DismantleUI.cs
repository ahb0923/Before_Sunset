using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DismantleUI : MonoBehaviour, ICloseableUI
{
    [SerializeField] private TextMeshProUGUI _targetNameText;
    [SerializeField] private TextMeshProUGUI _builtTimeText;
    
    [SerializeField] private Button _inGameButton;
    [SerializeField] private Button _dismantleButton;
    [SerializeField] private Button _cancelDismantleButton;
    [SerializeField] private Button _cancelButton;
    
    [SerializeField] private GameObject _slotArea;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private List<BuildingMaterialSlot> _slots = new List<BuildingMaterialSlot>();
    
    private RectTransform _rect;
    private BaseTower _selectedTower;
    public BaseTower SelectedTower() => _selectedTower;
    private Smelter _selectedSmelter;
    public Smelter SelectedSmelter() => _selectedSmelter;

    
    private const string TARGET_NAME_TEXT = "TargetNameText";
    private const string BUILT_TIME_TEXT = "BuiltTimeText";
    private const string IN_GAME_BUTTON = "UnBuildButton";
    private const string DISMENTLE_BUTTON = "DismantleButton";
    private const string CANCEL_DISMANTLE_BUTTON = "CancelDismantleButton";
    private const string CANCEL_BUTTON = "BackGroundCancelButton";
    private const string RECOVER_MATERIAL_SLOT_AREA = "RecoverMaterialSlotArea";
    private const string BUILDING_MATERIAL_SLOT = "Slots/BuildingMaterialSlot";

    private void Reset()
    {
        _targetNameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TARGET_NAME_TEXT);
        _builtTimeText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BUILT_TIME_TEXT);
        _inGameButton = Helper_Component.FindChildComponent<Button>(this.transform.parent, IN_GAME_BUTTON);
        _dismantleButton = Helper_Component.FindChildComponent<Button>(this.transform, DISMENTLE_BUTTON);
        _cancelDismantleButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_DISMANTLE_BUTTON);
        _cancelButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_BUTTON);
        _slotArea = Helper_Component.FindChildGameObjectByName(this.gameObject, RECOVER_MATERIAL_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(BUILDING_MATERIAL_SLOT);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _inGameButton.onClick.AddListener(DismantleMode);
        _dismantleButton.onClick.AddListener(Dismantle);
        _cancelDismantleButton.onClick.AddListener(Close);
        _cancelButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        CloseUI();
    }

    private void DismantleMode()
    {
        if (UIManager.Instance.UpgradeUI.isActiveAndEnabled) return;

        BuildManager.Instance.IsOnDestroy = !BuildManager.Instance.IsOnDestroy;
        //TODO UIManager에서 철거버튼 색상 바꿔주기
        InteractManager.Instance.SetCursorDestroyImage(BuildManager.Instance.IsOnDestroy);
    }

    private void Dismantle()
    {
        if (_selectedSmelter == null)
            _selectedTower.DestroyTower();
        else
            _selectedSmelter.DestroySmelter();
        Close();
    }

    public void OpenDismantleUI(BaseTower tower)
    {
        _selectedSmelter = null;
        
        if (!this.gameObject.activeSelf)
            UIManager.Instance.OpenUIClosingEveryUI(this);

        InitSlots(tower);
        SetSlot(tower);
        SetDismantleUI(tower);
    }

    public void OpenDismantleUI(Smelter Smelter)
    {
        _selectedTower = null;

        if (!this.gameObject.activeSelf)
            UIManager.Instance.OpenUIClosingEveryUI(this);

        InitSlots(Smelter);
        SetSlot(Smelter);
        SetDismantleUI(Smelter);
    }
    public void OpenDismantleUI(IInteractable obj)
    {
        if (obj is BaseTower){
            _selectedSmelter = null;
            BaseTower towerObj = obj as BaseTower;
            if (!this.gameObject.activeSelf)
                UIManager.Instance.OpenUIClosingEveryUI(this);

            InitSlots(towerObj);
            SetSlot(towerObj);
            SetDismantleUI(towerObj);

        }
        else if(obj is Smelter)
        {
            _selectedTower = null;
            Smelter smelterObj = obj as Smelter;
            if (!this.gameObject.activeSelf)
                UIManager.Instance.OpenUIClosingEveryUI(this);

            InitSlots(smelterObj);
            SetSlot(smelterObj);
            SetDismantleUI(smelterObj);
        }

        
    }


    public void Open()
    {
        throw new System.NotImplementedException();
    }

    public void Close()
    {
        if (_selectedTower!=null)
            _selectedTower.ui.OffAttackArea();
        //InteractManager.Instance.SetCursorDestroyImage(false);
        UIManager.Instance.CloseUI(this);
    }

    public void OpenUI()
    {
        _rect.OpenAtCenter();
    }

    public void CloseUI()
    {
        _rect.CloseAndRestore();
    }

    private void InitSlots(BaseTower tower)
    {
        _selectedTower = tower;

        int neededCount = tower.statHandler.AccumulatedCosts.Count;

        if (_slots.Count >= neededCount)
            return;

        int toAdd = neededCount - _slots.Count;

        for (int i = 0; i < toAdd; i++)
        {
            var slot = Instantiate(_slotPrefab, _slotArea.transform);
            var slotComponent = slot.GetComponent<BuildingMaterialSlot>();
            _slots.Add(slotComponent);
            slotComponent.InitIndex(_slots.Count - 1);
        }
    }
    
    private void InitSlots(Smelter smelter)
    {
        _selectedSmelter = smelter;

        int neededCount = smelter.smelterData.buildRequirements.Count;

        if (_slots.Count >= neededCount)
            return;

        int toAdd = neededCount - _slots.Count;

        for (int i = 0; i < toAdd; i++)
        {
            var slot = Instantiate(_slotPrefab, _slotArea.transform);
            var slotComponent = slot.GetComponent<BuildingMaterialSlot>();
            _slots.Add(slotComponent);
            slotComponent.InitIndex(_slots.Count - 1);
        }
    }

    private void SetSlot(BaseTower tower)
    {
        var costs = _selectedTower.statHandler.AccumulatedCosts.ToList();
        var ratio = _selectedTower.GetRefundRatio(_selectedTower.statHandler.CurrHp / _selectedTower.statHandler.MaxHp);

        for (int i = 0; i < costs.Count; i++)
        {
            var dataName = costs[i].Key;
            var dataAmount = (int)(costs[i].Value * ratio);

            if (i < _slots.Count)
            {
                _slots[i].ClearSlot();
                _slots[i].SetSlotDismantle(dataName, dataAmount);
            }
        }
       
        for (int i = costs.Count; i < _slots.Count; i++)
        {
            _slots[i].ClearSlot();
        }
    }
    
    private void SetSlot(Smelter smelter)
    {
        var costs = _selectedSmelter.smelterData.buildRequirements.ToList();
        var ratio = 0.9f;

        for (int i = 0; i < costs.Count; i++)
        {
            var dataName = costs[i].Key;
            var dataAmount = (int)(costs[i].Value * ratio);

            if (i < _slots.Count)
            {
                _slots[i].ClearSlot();
                _slots[i].SetSlotDismantle(dataName, dataAmount);
            }
        }

        for (int i = costs.Count; i < _slots.Count; i++)
        {
            _slots[i].ClearSlot();
        }
    }


    public void SetDismantleUI(BaseTower tower)
    {
        _targetNameText.text = tower.statHandler.TowerName;
    }

    public void SetDismantleUI(Smelter smelter)
    {
        _targetNameText.text = smelter.smelterData.smelterName;
    }
}
