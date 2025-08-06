using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour, ICloseableUI
{
    [SerializeField] private TextMeshProUGUI _targetNameText;
    [SerializeField] private TextMeshProUGUI _targetInfoText;
    [SerializeField] private TextMeshProUGUI _upgradeTargetNameText;
    [SerializeField] private TextMeshProUGUI _targetStatText;
    [SerializeField] private TextMeshProUGUI _upgradeStatText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _cancelButton;

    [SerializeField] private GameObject _slotArea;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private List<BuildingMaterialSlot> _slots = new List<BuildingMaterialSlot>();
    [SerializeField] private GameObject _maxUpgrade;
    [SerializeField] private GameObject _arrow;

    private RectTransform _rect;
    private BaseTower _selectedTower;
    public BaseTower SelectedTower ()=> _selectedTower;

    private const string TARGET_NAME_TEXT = "TargetNameText";
    private const string TARGET_INFO_TEXT = "TargetInfoText";
    private const string UPGRADE_TARGET_NAME_TEXT = "UpgradeTargetNameText";
    private const string TARGET_STAT_TEXT = "TargetStatText";
    private const string UPGRADE_STAT_TEXT = "UpgradeStatText";
    private const string UPGRADE_BUTTON = "UpgradeButton";
    private const string CANCEL_BUTTON = "BackGroundCancelButton";
    private const string UPGRADE_REQUIREMENT_SLOT_AREA = "UpgradeRequirementSlotArea";
    private const string BUILDING_MATERIAL_SLOT = "Slots/BuildingMaterialSlot";
    private const string MAX_UPGRADE = "MaxUpgrade";
    private const string ARROW = "Arrow";

    private void Reset()
    {
        _targetNameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TARGET_NAME_TEXT);
        _targetInfoText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TARGET_INFO_TEXT);
        _upgradeTargetNameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_TARGET_NAME_TEXT);
        _targetStatText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TARGET_STAT_TEXT);
        _upgradeStatText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_STAT_TEXT);
        _upgradeButton = Helper_Component.FindChildComponent<Button>(this.transform, UPGRADE_BUTTON);
        _cancelButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_BUTTON);
        _slotArea = Helper_Component.FindChildGameObjectByName(this.gameObject, UPGRADE_REQUIREMENT_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(BUILDING_MATERIAL_SLOT);
        _maxUpgrade = Helper_Component.FindChildGameObjectByName(this.gameObject, MAX_UPGRADE);
        _arrow = Helper_Component.FindChildGameObjectByName(this.gameObject, ARROW);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _upgradeButton.onClick.AddListener(Upgrade);
        _cancelButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        CloseUI();
    }

    private void Upgrade()
    {
        _selectedTower.statHandler.UpgradeTowerStat();
        InitSlots(_selectedTower);
        SetSlot(_selectedTower);
        SetUpgradeUI(_selectedTower);
    }

    public void OpenUpgradeUI(BaseTower tower)
    {
        if (!this.gameObject.activeInHierarchy)
            UIManager.Instance.OpenUIClosingEveryUI(this);
        
        InitSlots(tower);
        SetSlot(tower);
        SetUpgradeUI(tower);
    }

    public void Open()
    {
        throw new System.NotImplementedException();
    }

    public void Close()
    {
        ClearUI();
        UIManager.Instance.CloseUI(this);
    }

    public void OpenUI()
    {
        _rect.OpenAtCenter();
    }

    public void CloseUI()
    {
        if (_selectedTower != null)
            _selectedTower.ui.OffAttackArea();
        _rect.CloseAndRestore();
    }
   
    private void InitSlots(BaseTower tower)
    {
        _selectedTower = tower;
        if (tower.statHandler.NextupgradeID == null)
        {
            return;
        }
        else
        {
            var upgradeTowerData = DataManager.Instance.TowerData.GetById((int)tower.statHandler.NextupgradeID);

            if (_slots.Count >= upgradeTowerData.buildRequirements.Count)
            {
                return;
            }
            else
            {
                int num = upgradeTowerData.buildRequirements.Count - _slots.Count;
                for (int i = 0; i < num; i++)
                {
                    var slot = Instantiate(_slotPrefab, _slotArea.transform);
                    var slotComponent = slot.GetComponent<BuildingMaterialSlot>();
                    _slots.Add(slotComponent);
                    slotComponent.InitIndex(_slots.Count - 1);
                }
            }
        }
    }

    private void SetSlot(BaseTower tower)
    {
        if (tower.statHandler.NextupgradeID == null)
        {
            return;
        }
        else
        {
            var upgradeTowerData = DataManager.Instance.TowerData.GetById((int)tower.statHandler.NextupgradeID);

            List<KeyValuePair<string, int>> dataList = upgradeTowerData.buildRequirements.ToList();
            List<Item> items = InventoryManager.Instance.Inventory.Items.ToList();

            foreach (var slot in _slots)
            {
                slot.ClearSlot();

                if (slot.Index < dataList.Count)
                {
                    var dataName = dataList[slot.Index].Key;
                    var dataAmount = dataList[slot.Index].Value;

                    slot.SetSlotUpgrade(dataName, dataAmount, items);
                }
            }
        }
    }

    private void SetUpgradeUI(BaseTower tower)
    {
        if (tower.statHandler.NextupgradeID == null)
        {
            _targetNameText.text = tower.statHandler.TowerName;
            _targetInfoText.text = tower.statHandler.FlavorText;
            _upgradeTargetNameText.text = "";
            _targetStatText.text = $"MaxHP : {tower.statHandler.MaxHp}\n" +
                                   $"공격력 : {tower.statHandler.AttackPower}\n" +
                                   $"사거리 : {tower.statHandler.AttackRange}\n" +
                                   $"쿨타임 : {tower.statHandler.AttackSpeed}";
            _upgradeStatText.text = "";
            _maxUpgrade.SetActive(true);
            _arrow.SetActive(false);
        }
        else
        {
            var upgradeTowerData = DataManager.Instance.TowerData.GetById((int)tower.statHandler.NextupgradeID);
            
            _targetNameText.text = tower.statHandler.TowerName;
            _targetInfoText.text = tower.statHandler.FlavorText;
            _upgradeTargetNameText.text = upgradeTowerData.towerName;
            _targetStatText.text = $"MaxHP : {tower.statHandler.MaxHp}\n" +
                                   $"공격력 : {tower.statHandler.AttackPower}\n" +
                                   $"사거리 : {tower.statHandler.AttackRange}\n" +
                                   $"쿨타임 : {tower.statHandler.AttackSpeed}";
            _upgradeStatText.text = $"MaxHP : {tower.statHandler.MaxHp + upgradeTowerData.towerHp}\n" +
                                    $"공격력 : {tower.statHandler.AttackPower + upgradeTowerData.damage}\n" +
                                    $"사거리 : {tower.statHandler.AttackRange + upgradeTowerData.range}\n" +
                                    $"쿨타임 : {tower.statHandler.AttackSpeed + upgradeTowerData.aps}";
            _maxUpgrade.SetActive(false);
            _arrow.SetActive(true);
        }
    }
    
    private void ClearUI()
    {
        _targetNameText.text = "";
        _targetInfoText.text = "";
        _upgradeTargetNameText.text = "";
        _targetStatText.text = "";
        _upgradeStatText.text = "";
    }

}
