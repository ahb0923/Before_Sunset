using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotInventory : BaseInventory
{
    private List<QuickSlot> _quickSlots = new List<QuickSlot>();
    
    private const string ITEM_SLOT_AREA = "QuickSlotArea";
    private const string ITEM_SLOT_PREFAB = "QuickSlot";
    
    private void Reset()
    {
        _inventory = this.gameObject;
        _itemSlotContainer = _inventory.transform.Find(ITEM_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(ITEM_SLOT_PREFAB);
    }

    protected override void Awake()
    {
        if (_inventory == null)
        {
            Debug.Log("Quick Slot Inventory is empty");
        }
        else
        {
            if (!_inventory.activeSelf)
            {
                _inventory.SetActive(true);
            }
        }

        InitSlots();
    }
    
    protected override void InitSlots()
    {
        for (int i = 0; i < 9; i++)
        {
            var slot = Instantiate(_slotPrefab, _itemSlotContainer);
            _quickSlots.Add(slot.GetComponent<QuickSlot>());
        }
    }

    public override void AddItem(Item item)
    {
        
    }
    
    public override void RefreshUI()
    {
        foreach (var slot in _itemSlots)
        {
            slot.UpdateUI();
        }
    }
}