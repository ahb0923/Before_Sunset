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

    private const string UPGRADE_CONTENT_TEXT = "UpgradeText";
    private const string CURRENT_LEVEL_TEXT = "CurrentLevelText";
    private const string CURRENT_VALUE_TEXT = "CurrentValueText";
    private const string NEXT_LEVEL_TEXT = "NextLevelText";
    private const string NEXT_VALUE_TEXT = "NextValueText";
    private const string UPGRADE_COST_TEXT = "UpgradeCostText";
    
    private const string UPGRADE_BUTTON = "UpgradeButton";

    private void Reset()
    {
        _upgradeContentText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_CONTENT_TEXT);
        _currentLevelText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, CURRENT_LEVEL_TEXT);
        _currentValueText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, CURRENT_VALUE_TEXT);
        _nextLevelText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, NEXT_LEVEL_TEXT);
        _nextValueText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, NEXT_VALUE_TEXT);
        _upgradeCostText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_COST_TEXT);
        
        _upgradeButton = Helper_Component.FindChildComponent<Button>(this.transform, UPGRADE_BUTTON);
    }
}
