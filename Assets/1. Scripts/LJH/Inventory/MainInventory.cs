using UnityEngine;
using UnityEngine.Serialization;

public class MainInventory : BaseInventory
{
    [SerializeField] private int _numberOfQuickSlots = 9;
    [SerializeField] private int _numberOfItemSlots = 20;
    [SerializeField] private Transform _quickSlotContainer;
    
    private const string QUICK_SLOT_AREA = "QuickSlotArea";
    private const string ITEM_SLOT_AREA = "ItemSlotArea";
    private const string ITEM_SLOT_PREFAB = "ItemSlot";
    
    private void Reset()
    {
        _inventory = this.gameObject;
        _quickSlotContainer = _inventory.transform.Find(QUICK_SLOT_AREA);
        _itemSlotContainer = _inventory.transform.Find(ITEM_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(ITEM_SLOT_PREFAB);
    }
    
    protected override void InitSlots()
    {
        for (int i = 0; i < _numberOfQuickSlots; i++)
        {
            var slot = Instantiate(_slotPrefab, _quickSlotContainer);
            _itemSlots.Add(slot.GetComponent<ItemSlot>());
        }
        
        for (int i = 0; i < _numberOfItemSlots; i++)
        {
            var slot = Instantiate(_slotPrefab, _itemSlotContainer);
            _itemSlots.Add(slot.GetComponent<ItemSlot>());
        }
    }

    public override bool AddItem(Item item)
    {
        foreach (var slot in _itemSlots)
        {
            if (slot.CanStack(item))
            {
                slot.StackItem(item);
                return true;
            }
        }
        
        foreach (var slot in _itemSlots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(item);
                return true;
            }
        }
        
        Debug.Log("Inventory is full.");
        return false;
    }

    public override void RefreshUI()
    {
        foreach (var slot in _itemSlots)
        {
            slot.UpdateUI();
        }
    }
}
