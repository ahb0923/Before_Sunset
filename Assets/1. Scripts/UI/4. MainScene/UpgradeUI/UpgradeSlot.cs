using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _upgradeContentText;
    [SerializeField] private TextMeshProUGUI _currentLevelText;
    [SerializeField] private TextMeshProUGUI _currentValueText;
    [SerializeField] private TextMeshProUGUI _nextLevelText;
    [SerializeField] private TextMeshProUGUI _nextValueText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private GameObject _maxUpgradeText;
    [SerializeField] private GameObject _arrow;
    
    public string UpgradeName { get; private set; }

    private const string UPGRADE_CONTENT_TEXT = "UpgradeText";
    private const string CURRENT_LEVEL_TEXT = "CurrentLevelText";
    private const string CURRENT_VALUE_TEXT = "CurrentValueText";
    private const string NEXT_LEVEL_TEXT = "NextLevelText";
    private const string NEXT_VALUE_TEXT = "NextValueText";
    private const string UPGRADE_COST_TEXT = "UpgradeCostText";
    private const string UPGRADE_BUTTON = "UpgradeButton";
    private const string MAX_UPGRADE_TEXT = "MaxUpgradeText";
    private const string ARROW = "Arrow";

    private void Reset()
    {
        _upgradeContentText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_CONTENT_TEXT);
        _currentLevelText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, CURRENT_LEVEL_TEXT);
        _currentValueText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, CURRENT_VALUE_TEXT);
        _nextLevelText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, NEXT_LEVEL_TEXT);
        _nextValueText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, NEXT_VALUE_TEXT);
        _upgradeCostText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_COST_TEXT);
        _upgradeButton = Helper_Component.FindChildComponent<Button>(this.transform, UPGRADE_BUTTON);
        _maxUpgradeText = Helper_Component.FindChildGameObjectByName(this.gameObject, MAX_UPGRADE_TEXT);
        _arrow = Helper_Component.FindChildGameObjectByName(this.gameObject, ARROW);
    }
    
    private void Awake()
    {
        _upgradeButton.onClick.AddListener(VirtualUpgrade);
        EnableUpgrade();
    }

    public void SetName(string upgradeName)
    {
        UpgradeName = upgradeName;
    }

    public void SetSlot(int id)
    {
        var data = DataManager.Instance.UpgradeData.GetById(id);
        var nextData = FindNextUpgrade(id);
        
        _upgradeContentText.text = data.upgradeName;
        _currentLevelText.text = $"Lv. 0{data.level}";

        if (nextData != null)
        {
            EnableUpgrade();
            _nextLevelText.text = $"Lv. 0{nextData.level}";
            _upgradeCostText.text = nextData.essenceCost.ToString();

            switch (data.statType)
            {
                case UPGRADE_TYPE.MiningSpeed:
                case UPGRADE_TYPE.DropRate:
                    _currentValueText.text = $"{data.increaseRate} %";
                    _nextValueText.text = $"+{nextData.increaseRate - data.increaseRate} %";
                    break;

                case UPGRADE_TYPE.SightRange:
                    _currentValueText.text = $"{data.increaseRate} 칸";
                    _nextValueText.text = $"+{nextData.increaseRate - data.increaseRate} 칸";
                    break;

                case UPGRADE_TYPE.MoveSpeed:
                case UPGRADE_TYPE.HP:
                    _currentValueText.text = $"{data.increaseRate}";
                    _nextValueText.text = $"+{nextData.increaseRate - data.increaseRate}";
                    break;

                case UPGRADE_TYPE.HpRegen:
                    _currentValueText.text = $"+{data.increaseRate}";
                    _nextValueText.text = $"+{nextData.increaseRate - data.increaseRate}";
                    break;

                case UPGRADE_TYPE.DashCooldown:
                    _currentValueText.text = $"{data.increaseRate} 초";
                    _nextValueText.text = $"-0.2 초";  // 추후 데이터가 추가될경우 double 형 연산 혹은 반올림 소수 한자리까지 적용할 것
                    break;


                default:
                    Debug.LogWarning("정의되지 않은 업그레이드 타입입니다.");
                    break;
            }
        }
        else
        {
            DisableUpgrade();
            switch (data.statType)
            {

                case UPGRADE_TYPE.MiningSpeed:
                case UPGRADE_TYPE.DropRate:
                    _currentValueText.text = $"{data.increaseRate} %";
                    break;

                case UPGRADE_TYPE.SightRange:
                    _currentValueText.text = $"{data.increaseRate} 칸";
                    break;

                case UPGRADE_TYPE.MoveSpeed:
                case UPGRADE_TYPE.HP:
                    _currentValueText.text = $"{data.increaseRate}";
                    break;

                case UPGRADE_TYPE.HpRegen:
                    _currentValueText.text = $"+{data.increaseRate}";
                    break;

                case UPGRADE_TYPE.DashCooldown:
                    _currentValueText.text = $"{data.increaseRate} 초";
                    break;
                default:
                    Debug.LogWarning("정의되지 않은 업그레이드 타입입니다.");
                    break;
            }
        }
    }
    
    private UpgradeDatabase FindNextUpgrade(int id)
    {
        var nextUpgrade = DataManager.Instance.UpgradeData.GetById(id+1);
        if (nextUpgrade == null)
            return null;
        return nextUpgrade;
    }
    
    private void VirtualUpgrade()
    {
        var id = UpgradeManager.Instance.VirtualUpgrade[UpgradeName];
        var nextId = DataManager.Instance.UpgradeData.GetById(id+1);
        
        if (nextId == null)
            return;

        if (UpgradeManager.Instance.VirtualEssence >= nextId.essenceCost)
        {
            UpgradeManager.Instance.UseVirtualEssence(nextId.essenceCost);
            UpgradeManager.Instance.ChangeVirtualUpgrade(UpgradeName, nextId.id);
            UIManager.Instance.UpgradeUI.UpgradeChanged();
            UIManager.Instance.UpgradeUI.Refresh();
            RefreshSlot(nextId.id);
        }
        else
        {
            ToastManager.Instance.ShowToast("정수가 부족합니다.");
        }
    }

    public void RefreshSlot(int id)
    {
        SetSlot(id);
    }

    private void DisableUpgrade()
    {
        _upgradeButton.gameObject.SetActive(false);
        _arrow.gameObject.SetActive(false);
        _maxUpgradeText.gameObject.SetActive(true);
    }

    private void EnableUpgrade()
    {
        _upgradeButton.gameObject.SetActive(true);
        _arrow.gameObject.SetActive(true);
        _maxUpgradeText.gameObject.SetActive(false);
    }
}
