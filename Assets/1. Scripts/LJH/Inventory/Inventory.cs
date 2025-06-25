using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    private const string SORT_BUTTON = "SortButton";

    private void Start()
    {
        InventoryManager.Instance.Init(this);
        Button sortButton = UtilityLJH.FindChildComponent<Button>(this.transform, SORT_BUTTON);
        sortButton.onClick.AddListener(Sort);
    }

    private void Update()
    {
        //아이템 획득 키
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddItem(InventoryManager.Instance.itemData.CreateItem());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            AddItem(InventoryManager.Instance.itemData2.CreateItem());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            AddItem(InventoryManager.Instance.itemData3.CreateItem());
        }
        
        InventoryManager.Instance.InventoryUI.ToggleInventory();
        InventoryManager.Instance.QuickSlotInventoryUI.ToggleInventory();
    }

    /// <summary>
    /// 인벤토리에 아이템 추가 메서드
    /// </summary>
    /// <param name="item">추가 할 아이템</param>
    public void AddItem(Item item)
    {
        List<ItemSlot> _itemSlots = InventoryManager.Instance.InventoryUI.itemSlots;
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
    /// 인벤토리 내 아이템 정렬 메서드
    /// </summary>
    public void Sort()
    {
        List<ItemSlot> _itemSlots = InventoryManager.Instance.InventoryUI.itemSlots;
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
