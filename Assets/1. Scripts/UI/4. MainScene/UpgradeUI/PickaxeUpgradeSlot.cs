using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickaxeUpgradeSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _upgradeContentText;
    [SerializeField] private TextMeshProUGUI _currentPickaxeText;
    [SerializeField] private TextMeshProUGUI _nextPickaxeText;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private Image _currentPickaxeImage;
    [SerializeField] private Image _nextPickaxeImage;
    [SerializeField] private Image _ingotImage;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private GameObject _maxUpgrade;

    private int _currentId;

    private const string UPGRADE_CONTENT_TEXT = "UpgradeText";
    private const string CURRENT_PICKAXE_TEXT = "CurrentPickaxeText";
    private const string NEXT_PICKAXE_TEXT = "NextPickaxeText";
    private const string UPGRADE_COST_TEXT = "UpgradeCostText";
    private const string CURRENT_PICKAXE_IMAGE = "CurrentPickaxeImage";
    private const string NEXT_PICKAXE_IMAGE = "NextPickaxeImage";
    private const string INGOT_IMAGE = "IngotImage";
    private const string UPGRADE_BUTTON = "UpgradeButton";
    private const string MAX_UPGRADE = "MaxUpgrade";
    private const string ARROW = "Arrow";

    private void Reset()
    {
        _upgradeContentText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_CONTENT_TEXT);
        _currentPickaxeText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, CURRENT_PICKAXE_TEXT);
        _nextPickaxeText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, NEXT_PICKAXE_TEXT);
        _upgradeCostText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, UPGRADE_COST_TEXT);
        _currentPickaxeImage = Helper_Component.FindChildComponent<Image>(this.transform, CURRENT_PICKAXE_IMAGE);
        _nextPickaxeImage = Helper_Component.FindChildComponent<Image>(this.transform, NEXT_PICKAXE_IMAGE);
        _ingotImage = Helper_Component.FindChildComponent<Image>(this.transform, INGOT_IMAGE);
        _upgradeButton = Helper_Component.FindChildComponent<Button>(this.transform, UPGRADE_BUTTON);
        _arrow = Helper_Component.FindChildGameObjectByName(this.gameObject, ARROW);
        _maxUpgrade = Helper_Component.FindChildGameObjectByName(this.gameObject, MAX_UPGRADE);
    }
    
    private void Awake()
    {
        _upgradeButton.onClick.AddListener(Upgrade);
        EnableUpgrade();
    }
    
    public void SetSlot(int id)
    {
        _currentId = id;
        var data = DataManager.Instance.EquipmentData.GetById(id);
        if (data.nextUpgradeId != 0)
        {
            EnableUpgrade();
            var nextData = DataManager.Instance.EquipmentData.GetById(data.nextUpgradeId);

            _upgradeContentText.text = data.itemName;
            _currentPickaxeText.text = data.itemName;
            _nextPickaxeText.text = nextData.itemName;

            _currentPickaxeImage.sprite = DataManager.Instance.EquipmentData.GetSpriteById(id);
            _nextPickaxeImage.sprite = DataManager.Instance.EquipmentData.GetSpriteById(data.nextUpgradeId);

            var requirement = nextData.upgradeRequirements.ToList();
            var material = DataManager.Instance.MineralData.GetByName(requirement[0].Key);
            _ingotImage.sprite = DataManager.Instance.MineralData.GetSpriteById(material.id);
            _upgradeCostText.text = requirement[0].Value.ToString();
        }
        else
        {
            DisableUpgrade();
            _upgradeContentText.text = data.itemName;
            _currentPickaxeText.text = data.itemName;
            _currentPickaxeImage.sprite = DataManager.Instance.EquipmentData.GetSpriteById(id);
        }
    }
    
    private void Upgrade()
    {
        var data = DataManager.Instance.EquipmentData.GetById(_currentId);
        var nextData = data;
        if (data.nextUpgradeId != 0)
            nextData = DataManager.Instance.EquipmentData.GetById(data.nextUpgradeId);
        var requirement = nextData.upgradeRequirements.ToList();
        var material = DataManager.Instance.MineralData.GetByName(requirement[0].Key);

        if (InventoryManager.Instance.IsEnoughMaterial(nextData))
        {
            InventoryManager.Instance.Inventory.UseItem(material.id, requirement[0].Value);
            InventoryManager.Instance.Inventory.SetPickaxe(nextData);
            InventoryManager.Instance.Inventory.RefreshPickaxeSlots();
            RefreshSlot(nextData.id);
        }
        else
        {
            ToastManager.Instance.ShowToast("재료가 부족합니다.");
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
        _maxUpgrade.gameObject.SetActive(true);
    }

    private void EnableUpgrade()
    {
        _upgradeButton.gameObject.SetActive(true);
        _arrow.gameObject.SetActive(true);
        _maxUpgrade.gameObject.SetActive(false);
    }
}
