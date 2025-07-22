using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("Upgrade UI")]
    [SerializeField] private TextMeshProUGUI _essenceText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _backGroundButton;
    
    [Header("UpgradeSlot")]
    [SerializeField] private GameObject _playerUpgrade;
    [SerializeField] private GameObject _coreUpgrade;
    [SerializeField] private GameObject _pickaxeUpgrade;
    [SerializeField] private GameObject _upgradeSlotPrefab;
    [SerializeField] private Transform _playerUpgradeContainer;
    [SerializeField] private Transform _coreUpgradeContainer;
    [SerializeField] private Transform _pickaxeUpgradeContainer;

    private RectTransform _rect;
    
    private const string ESSENCE_TEXT = "EssenceAmountText";
    private const string COST_TEXT = "ResetCostText";
    private const string RESET_BUTTON = "ResetButton";
    private const string CLOSE_BUTTON = "CloseUpgradeButton";
    private const string BACK_GROUND_BUTTON = "BackGroundCancelButton";
    
    private const string PLAYER_UPGRADE = "PlayerUpgrade";
    private const string CORE_UPGRADE = "CoreUpgrade";
    private const string PICKAXE_UPGRADE = "PickaxeUpgrade";
    private const string UPGRADE_SLOT_PREFAB = "Slots/UpgradeSlot";
    private const string UPGRADE_CONTAINER = "UpgradeContainer";
    
    private void Reset()
    {
        _essenceText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, ESSENCE_TEXT);
        _costText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, COST_TEXT);
        _resetButton = Helper_Component.FindChildComponent<Button>(this.transform, RESET_BUTTON);
        _closeButton = Helper_Component.FindChildComponent<Button>(this.transform, CLOSE_BUTTON);
        _backGroundButton = Helper_Component.FindChildComponent<Button>(this.transform, BACK_GROUND_BUTTON);
        
        _playerUpgrade = Helper_Component.FindChildGameObjectByName(this.gameObject, PLAYER_UPGRADE);
        _coreUpgrade = Helper_Component.FindChildGameObjectByName(this.gameObject, CORE_UPGRADE);
        _pickaxeUpgrade = Helper_Component.FindChildGameObjectByName(this.gameObject, PICKAXE_UPGRADE);
        _upgradeSlotPrefab = Resources.Load<GameObject>(UPGRADE_SLOT_PREFAB);
        _playerUpgradeContainer = Helper_Component.FindChildComponent<Transform>(_playerUpgrade.transform, UPGRADE_CONTAINER);
        _coreUpgradeContainer = Helper_Component.FindChildComponent<Transform>(_coreUpgrade.transform, UPGRADE_CONTAINER);
        _pickaxeUpgradeContainer = Helper_Component.FindChildComponent<Transform>(_pickaxeUpgrade.transform, UPGRADE_CONTAINER);
    }

    private void Awake()
    {
        _resetButton.onClick.AddListener(ResetUpgrade);
        _closeButton.onClick.AddListener(CloseUpgrade);
        _backGroundButton.onClick.AddListener(CloseUpgrade);
    }

    private void ResetUpgrade()
    {
        
    }

    public void OpenUpgrade()
    {
        _rect.OpenAtCenter();
    }
    
    private void CloseUpgrade()
    {
        _rect.CloseAndRestore();
    }

    private void InitSlot()
    {
        
    }
    
    
}
