using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DismantleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _targetNameText;
    [SerializeField] private TextMeshProUGUI _builtTimeText;
    
    [SerializeField] private Button _dismantleButton;
    [SerializeField] private Button _cancelDismantleButton;
    [SerializeField] private Button _cancelButton;
    
    [SerializeField] private GameObject _slotArea;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private List<BuildingMaterialSlot> _slots = new List<BuildingMaterialSlot>();
    
    private RectTransform _rect;
    
    private const string TARGET_NAME_TEXT = "TargetNameText";
    private const string BUILT_TIME_TEXT = "BuiltTimeText";
    private const string DISMENTLE_BUTTON = "DismantleButton";
    private const string CANCEL_DISMANTLE_BUTTON = "CancelDismantleButton";
    private const string CANCEL_BUTTON = "BackGroundCancelButton";
    private const string RECOVER_MATERIAL_SLOT_AREA = "RecoverMaterialSlotArea";
    private const string BUILDING_MATERIAL_SLOT = "BuildingMaterialSlot";

    private void Reset()
    {
        _targetNameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TARGET_NAME_TEXT);
        _builtTimeText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BUILT_TIME_TEXT);
        _dismantleButton = Helper_Component.FindChildComponent<Button>(this.transform, DISMENTLE_BUTTON);
        _cancelDismantleButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_DISMANTLE_BUTTON);
        _cancelButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_BUTTON);
        _slotArea = Helper_Component.FindChildGameObjectByName(this.gameObject, RECOVER_MATERIAL_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(BUILDING_MATERIAL_SLOT);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _dismantleButton.onClick.AddListener(Dismantle);
        _cancelDismantleButton.onClick.AddListener(Cancel);
        _cancelButton.onClick.AddListener(Cancel);
    }

    private void Dismantle()
    {
        // 타워 해체 매서드
    }

    private void Cancel()
    {
        _targetNameText.text = "";
        _builtTimeText.text = "";
        Close();
    }

    public void OpenDismantleUI(TowerData data)
    {
        _rect.OpenAtCenter();
        InitSlots(data);
        SetSlot(data);
        SetDismantleUI(data);
    }

    private void Close()
    {
        _rect.CloseAndRestore();
    }

    private void InitSlots(TowerData data)
    {
        if (_slots.Count >= data.buildRequirements.Count)
        {
            return;
        }
        else
        {
            int num = data.buildRequirements.Count - _slots.Count;
            for (int i = 0; i < num; i++)
            {
                var slot = Instantiate(_slotPrefab, _slotArea.transform);
                var slotComponent = slot.GetComponent<BuildingMaterialSlot>();
                _slots.Add(slotComponent);
                slotComponent.InitIndex(i);
            }
        }
    }
    
    private void SetSlot(TowerData data)
    {
        var upgradeTowerData = DataManager.Instance.TowerData.GetById(data.nextUpgradeId);
        
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
    
    public void SetDismantleUI(TowerData data)
    {
        _targetNameText.text = data.towerName;
    }
    
    // TODO 타워 설치된 시간 표시 어디서 가져오지 (?)
}
