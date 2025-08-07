using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour, ICloseableUI
{
    [Header("Upgrade UI")]
    [SerializeField] private TextMeshProUGUI _essenceText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _fixButton;
    [SerializeField] private Button _backGroundButton;
    [SerializeField] private string _askText = "적용되지 않은 업그레이드가 있습니다.\n나가면 모든 변경사항이 사라집니다.";
    
    [Header("UpgradeSlot")]
    [SerializeField] private GameObject _playerUpgrade;
    [SerializeField] private GameObject _coreUpgrade;
    [SerializeField] private GameObject _upgradeSlotPrefab;
    [SerializeField] private Transform _playerUpgradeContainer;
    [SerializeField] private Transform _coreUpgradeContainer;
    [SerializeField] private List<UpgradeSlot> _playerUpgradeSlots = new List<UpgradeSlot>();
    [SerializeField] private List<UpgradeSlot> _coreUpgradeSlots = new List<UpgradeSlot>();
    [SerializeField] private PickaxeUpgradeSlot _pickaxeUpgradeSlot;

    private RectTransform _rect;
    public bool IsChange { get; private set; } = false;
    
    private const string ESSENCE_TEXT = "EssenceAmountText";
    private const string COST_TEXT = "ResetCostText";
    private const string RESET_BUTTON = "ResetButton";
    private const string CLOSE_BUTTON = "CloseUpgradeButton";
    private const string RETURN_BUTTON = "ReturnButton";
    private const string FIX_BUTTON = "FixButton";
    private const string BACK_GROUND_BUTTON = "BackGroundCancelButton";
    
    private const string PLAYER_UPGRADE = "PlayerUpgrade";
    private const string CORE_UPGRADE = "CoreUpgrade";
    private const string PICKAXE_UPGRADE = "PickaxeUpgradeSlot";
    private const string UPGRADE_SLOT_PREFAB = "Slots/UpgradeSlot";
    private const string UPGRADE_CONTAINER = "UpgradeContainer";
    
    private void Reset()
    {
        _essenceText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, ESSENCE_TEXT);
        _costText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, COST_TEXT);
        _resetButton = Helper_Component.FindChildComponent<Button>(this.transform, RESET_BUTTON);
        _closeButton = Helper_Component.FindChildComponent<Button>(this.transform, CLOSE_BUTTON);
        _returnButton = Helper_Component.FindChildComponent<Button>(this.transform, RETURN_BUTTON);
        _fixButton = Helper_Component.FindChildComponent<Button>(this.transform, FIX_BUTTON);
        _backGroundButton = Helper_Component.FindChildComponent<Button>(this.transform, BACK_GROUND_BUTTON);
        
        _playerUpgrade = Helper_Component.FindChildGameObjectByName(this.gameObject, PLAYER_UPGRADE);
        _coreUpgrade = Helper_Component.FindChildGameObjectByName(this.gameObject, CORE_UPGRADE);
        _upgradeSlotPrefab = Resources.Load<GameObject>(UPGRADE_SLOT_PREFAB);
        _playerUpgradeContainer = Helper_Component.FindChildComponent<Transform>(_playerUpgrade.transform, UPGRADE_CONTAINER);
        _coreUpgradeContainer = Helper_Component.FindChildComponent<Transform>(_coreUpgrade.transform, UPGRADE_CONTAINER);
        _pickaxeUpgradeSlot = Helper_Component.FindChildComponent<PickaxeUpgradeSlot>(this.transform, PICKAXE_UPGRADE);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _resetButton.onClick.AddListener(ResetUpgrade);
        _closeButton.onClick.AddListener(CloseUpgrade);
        _returnButton.onClick.AddListener(ReturnUpgrade);
        _fixButton.onClick.AddListener(FixUpgrade);
        _backGroundButton.onClick.AddListener(CloseUpgrade);
    }

    private void Start()
    {
        InitSlot();
        CloseUI();
    }

    private List<KeyValuePair<string, int>> GetListOfUpgrade(Dictionary<string, int> dictionary, UPGRADE_CATEGORY category)
    {
        return dictionary
            .Where(pair => DataManager.Instance.UpgradeData.GetById(pair.Value).category == category)
            .ToList();
    }

    private void InitSlot()
    {
        var dic = UpgradeManager.Instance.FixedUpgrade;
        
        var playerUpgrade = GetListOfUpgrade(dic, UPGRADE_CATEGORY.Player);
        for (int i = 0; i < playerUpgrade.Count; i++)
        {
            var go = Instantiate(_upgradeSlotPrefab, _playerUpgradeContainer);
            var slot = go.GetComponent<UpgradeSlot>();
            _playerUpgradeSlots.Add(slot);
            slot.SetName(playerUpgrade[i].Key);
            slot.SetSlot(playerUpgrade[i].Value);
        }
        
        var coreUpgrade = GetListOfUpgrade(dic, UPGRADE_CATEGORY.Core);
        for (int i = 0; i < coreUpgrade.Count; i++)
        {
            var go = Instantiate(_upgradeSlotPrefab, _coreUpgradeContainer);
            var slot = go.GetComponent<UpgradeSlot>();
            _coreUpgradeSlots.Add(slot);
            slot.SetName(coreUpgrade[i].Key);
            slot.SetSlot(coreUpgrade[i].Value);
        }

        var id = InventoryManager.Instance.Inventory.Pickaxe.Data.id;
        _pickaxeUpgradeSlot.SetSlot(id);
    }
    
    private void CloseUpgrade()
    {
        if (IsChange)
        {
            UIManager.Instance.AskPopUpUI.Open(_askText,Close,null);
        }
        else
        {
            Close();
        }
    }

    public void Open()
    {
        UIManager.Instance.OpenUIClosingEveryUI(this);
    }
    
    public void Close()
    {
        UIManager.Instance.CloseUI(this);
    }

    public void OpenUI()
    {
        UpgradeManager.Instance.SetVirtualUpgrade();
        UpgradeManager.Instance.SetVirtualEssence();
        TimeManager.Instance.PauseGame(true);
        
        Refresh();
        RefreshSlot();

        _rect.OpenAtCenter();
    }

    public void CloseUI()
    {
        UpgradeManager.Instance.DiscardVirtualUpgrade();
        UpgradeManager.Instance.SetVirtualEssence();
        TimeManager.Instance.PauseGame(false);
        
        _rect.CloseAndRestore();
    }

    private void ResetUpgrade()
    {
        if (IsChange)
        {
            ToastManager.Instance.ShowToast("미적용된 업그레이드가 있습니다.");
            return;
        }
        
        if (IsDicEqual(UpgradeManager.Instance.FixedUpgrade, UpgradeManager.Instance.BaseUpgrade))
        {
            ToastManager.Instance.ShowToast("이미 초기 상태입니다.");
            return;
        }
        
        if (UpgradeManager.Instance.VirtualEssence < UpgradeManager.Instance.ResetCost)
        {
            ToastManager.Instance.ShowToast("정수가 부족합니다.");
            return;
        }
        
        UpgradeManager.Instance.FixResetCounter();
        UpgradeManager.Instance.InitUpgrade();
        UpgradeManager.Instance.DiscardVirtualUpgrade();
        UpgradeManager.Instance.SetVirtualUpgrade();
        UpgradeManager.Instance.SetVirtualEssence();
        
        Refresh();
        RefreshSlot();
        UIManager.Instance.EssenceUI.Refresh();
        ToastManager.Instance.ShowToast("업그레이드 초기화 완료!");
    }
    
    private bool IsDicEqual(Dictionary<string, int> a, Dictionary<string, int> b)
    {
        if (a.Count != b.Count) return false;

        foreach (var kv in a)
        {
            if (!b.TryGetValue(kv.Key, out var value) || kv.Value != value)
                return false;
        }
        return true;
    }

    private void ReturnUpgrade()
    {
        if (IsChange)
        {
            UpgradeManager.Instance.DiscardVirtualUpgrade();
            UpgradeManager.Instance.SetVirtualUpgrade();
            UpgradeManager.Instance.SetVirtualEssence();

            Refresh();
            RefreshSlot();
            IsChange = false;
            ToastManager.Instance.ShowToast("되돌리기 완료!");
        }
        else
        {
            ToastManager.Instance.ShowToast("되돌릴 업그레이드가 없습니다.");
        }
    }

    private void FixUpgrade()
    {
        if (IsChange)
        {
            UpgradeManager.Instance.FixUpgrade();
            UpgradeManager.Instance.FixEssence();
            UpgradeManager.Instance.ApplyAllUpgradesToAll();
            IsChange = false;
            UIManager.Instance.EssenceUI.Refresh();
            ToastManager.Instance.ShowToast("적용하기 완료!");
        }
        else
        {
            ToastManager.Instance.ShowToast("적용할 업그레이드가 없습니다.");
        }
    }

    private void RefreshSlot()
    {
        var dic = UpgradeManager.Instance.VirtualUpgrade;
        
        var playerUpgrade = GetListOfUpgrade(dic, UPGRADE_CATEGORY.Player);
        for (int i = 0; i < _playerUpgradeSlots.Count; i++)
        {
            _playerUpgradeSlots[i].RefreshSlot(playerUpgrade[i].Value);
        }
        
        var coreUpgrade = GetListOfUpgrade(dic, UPGRADE_CATEGORY.Core);
        for (int i = 0; i < _coreUpgradeSlots.Count; i++)
        {
            _coreUpgradeSlots[i].RefreshSlot(coreUpgrade[i].Value);
        }
    }

    public void Refresh()
    {
        _essenceText.text = UpgradeManager.Instance.VirtualEssence.ToString();
        _costText.text = UpgradeManager.Instance.ResetCost.ToString();
    }

    public void UpgradeChanged()
    {
        IsChange = true;
    }
}
