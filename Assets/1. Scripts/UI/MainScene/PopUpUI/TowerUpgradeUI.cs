using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour
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

    private RectTransform _rect;
    private BaseTower _selectedTower;


    private const string TARGET_NAME_TEXT = "TargetNameText";
    private const string TARGET_INFO_TEXT = "TargetInfoText";
    private const string UPGRADE_TARGET_NAME_TEXT = "UpgradeTargetNameText";
    private const string TARGET_STAT_TEXT = "TargetStatText";
    private const string UPGRADE_STAT_TEXT = "UpgradeStatText";
    private const string UPGRADE_BUTTON = "UpgradeButton";
    private const string CANCEL_BUTTON = "BackGroundCancelButton";
    private const string UPGRADE_REQUIREMENT_SLOT_AREA = "UpgradeRequirementSlotArea";
    private const string BUILDING_MATERIAL_SLOT = "Slots/BuildingMaterialSlot";

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
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _upgradeButton.onClick.AddListener(Upgrade);
        _cancelButton.onClick.AddListener(CloseUpgradeUI);
    }

    private void Upgrade()
    {
        // 타워업그레이드 메서드
        _selectedTower.statHandler.UpgradeTowerStat();
    }
    /*
    public void OpenUpgradeUI(TowerDatabase data)
    {
        _rect.OpenAtCenter();
        InitSlots(data);
        SetSlot(data);
        SetUpgradeUI(data);
    }*/

    public void OpenUpgradeUI(BaseTower tower)
    {
        _rect.OpenAtCenter();
        InitSlots(tower);
        SetSlot(tower);
        SetUpgradeUI(tower);
    }



    public void CloseUpgradeUI()
    {
        ClearUI();
        _rect.CloseAndRestore();
    }
    /*
    private void InitSlots(TowerDatabase data)
    {
        var upgradeTowerData = DataManager.Instance.TowerData.GetById((int)data.nextUpgradeId);
        
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
                slotComponent.InitIndex(i);
            }
        }
    }*/
    private void InitSlots(BaseTower tower)
    {
        _selectedTower = tower;
        if (tower.statHandler.NextupgradeID == null)
        {
            Debug.Log("다음 업그레이드가 없음!");
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
                    slotComponent.InitIndex(i);
                }
            }
        }
            
    }
    /*
    private void SetSlot(TowerDatabase data)
    {
        var upgradeTowerData = DataManager.Instance.TowerData.GetById((int)data.nextUpgradeId);
        
        List<KeyValuePair<string, int>> dataList = upgradeTowerData.buildRequirements.ToList();
        List<Item> items = InventoryManager.Instance.Inventory.Items.ToList();
        
        foreach (var slot in _slots)
        {
            slot.ClearSlot();

            if (slot.Index < dataList.Count)
            {
                var dataName = dataList[slot.Index].Key;
                var dataAmount = dataList[slot.Index].Value;
                
                slot.SetSlot(dataName, dataAmount, items);
            }
        }
    }*/
    private void SetSlot(BaseTower tower)
    {
        if (tower.statHandler.NextupgradeID == null)
        {
            Debug.Log("다음 업그레이드가 없음!");
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

                    slot.SetSlot(dataName, dataAmount, items);
                }
            }
        }
    }
    /*
    private void SetUpgradeUI(TowerDatabase data)
    {
        var upgradeTowerData = DataManager.Instance.TowerData.GetById((int)data.nextUpgradeId);
            
        _targetNameText.text = data.towerName;
        _targetInfoText.text = data.flavorText;
        _upgradeTargetNameText.text = upgradeTowerData.towerName;
        _targetStatText.text = $"MaxHP : {data.towerHp}\n" +
                               $"공격력 : {data.damage}\n" +
                               $"사거리 : {data.range}\n" +
                               $"쿨타임 : {data.aps}";
        _upgradeStatText.text = $"MaxHP : {data.towerHp + upgradeTowerData.towerHp}\n" +
                                $"공격력 : {data.damage + upgradeTowerData.damage}\n" +
                                $"사거리 : {data.range + upgradeTowerData.range}\n" +
                                $"쿨타임 : {data.aps + upgradeTowerData.aps}";
    }*/

    private void SetUpgradeUI(BaseTower tower)
    {
        if (tower.statHandler.NextupgradeID == null)
        {
            Debug.Log("다음 업그레이드가 없음!");
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
