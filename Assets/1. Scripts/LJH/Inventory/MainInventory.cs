using System;
using System.Collections.Generic;
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

    /// <summary>
    /// 인벤토리에 아이템 추가 메서드
    /// </summary>
    /// <param name="item">추가 할 아이템</param>
    public override void AddItem(Item item)
    {
        foreach (var slot in _itemSlots)
        {
            if (slot.CanStack(item))
            {
                slot.StackItem(item);
                return;
            }
        }
        
        foreach (var slot in _itemSlots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(item);
                
                if (item.Data.stackable)
                {
                    slot.StackItem(item);
                }
                return;
            }
        }
        
        Debug.Log("Inventory is full.");
    }

    /// <summary>
    /// 인벤토리 아이템 슬롯 새로고침 메서드
    /// </summary>
    public override void RefreshUI()
    {
        foreach (var slot in _itemSlots)
        {
            slot.UpdateUI();
        }
    }

    /// <summary>
    /// 인벤토리 내 아이템 정렬 메서드
    /// </summary>
    public void Sort()
    {
        List<Item> items = new List<Item>();

        foreach (var slot in _itemSlots)
        {
            if (!slot.IsEmpty)
            {
                items.Add(slot.CurrentItem);
            }
        }

        List<Item> mergedItems = new List<Item>();

        foreach (var item in items)
        {
            var noMax = mergedItems.Find(x => x.Data.id == item.Data.id && x.Data.stackable);

            if (noMax != null)
            {
                int result = noMax.stack + item.stack;
                int max = item.Data.maxStack;

                if (result <= max)
                {
                    noMax.stack = result;
                }
                else
                {
                    noMax.stack = max;

                    Item leftover = item;
                    leftover.stack = result - max;
                    mergedItems.Add(leftover);
                }
            }
            else
            {
                mergedItems.Add(item);
            }
        }

        mergedItems.Sort((a, b) =>
        {
            int result = a.Data.id.CompareTo(b.Data.id);
            if (result != 0)
            {
                return result;
            }
            
            bool isAMax = a.Data.stackable && a.stack == a.Data.maxStack;
            bool isBMax = b.Data.stackable && b.stack == b.Data.maxStack;
            
            if (isAMax && !isBMax) return -1;

            return 0;
        });

        foreach (var slot in _itemSlots)
        {
            slot.SetItem(null);
        }

        for (int i = 0; i < mergedItems.Count && i < _itemSlots.Count; i++)
        {
            _itemSlots[i].SetItem(mergedItems[i]);
        }
    }
}
