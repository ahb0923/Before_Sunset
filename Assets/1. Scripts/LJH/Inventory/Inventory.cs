using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    public Item Pickaxe { get; private set; }
    public Item[] Items { get; private set; } = new Item[29];
    
    [field:SerializeField] public InventoryUI InventoryUI { get; private set; }
    [field:SerializeField] public QuickSlotInventoryUI QuickSlotInventoryUI { get; private set; }

    private const string SORT_BUTTON = "SortButton";

    private void Reset()
    {
        InventoryUI = GetComponentInChildren<InventoryUI>();
        QuickSlotInventoryUI = GetComponentInChildren<QuickSlotInventoryUI>();
    }

    private void Awake()
    {
        InventoryManager.Instance.Init(this);
        Button sortButton = UtilityLJH.FindChildComponent<Button>(this.transform, SORT_BUTTON);
        sortButton.onClick.AddListener(Sort);
        Pickaxe = InventoryManager.Instance.pickaxeItemData.CreateItem();
    }

    private void Update()
    {
        //아이템 획득 키
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddItem(InventoryManager.Instance.itemData.CreateItem());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AddItem(InventoryManager.Instance.itemData2.CreateItem());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            AddItem(InventoryManager.Instance.itemData3.CreateItem());
        }
        
        InventoryUI.ToggleInventory();
        QuickSlotInventoryUI.ToggleInventory();
    }

    
    /// <summary>
    /// 인벤토리에 아이템 추가 메서드
    /// </summary>
    /// <param name="item">추가 할 아이템</param>
    public void AddItem(Item item)
    {
        if (item.Data.stackable)
        {
            AddStackableItem(item, item.Data.quantity);
        }
        else
        {
            AddNotStackableItem(item);
        }

        InventoryUI.RefreshUI(Items);
        QuickSlotInventoryUI.RefreshUI(Items);
    }

    private void AddStackableItem(Item item, int quantity)
    {
        Item savedItem = null;
        
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null || Items[i].IsMaxStack)
                continue;
            
            if (Items[i].Data.itemName == item.Data.itemName)
            {
                savedItem = Items[i];
                break;
            }
        }

        if (savedItem == null)
        {
            int emptyIndex = GetEmptySlotIndex();
            if (emptyIndex == -1)
            {
                Debug.LogWarning("추가 불가능");
            }
            else
            {
                item.stack += quantity;
                Items[emptyIndex] = item;
            }
        }
        // 기존에 겹칠수 있는 아이템이 있을 경우
        else
        {
            int max = savedItem.Data.maxStack;
            savedItem.stack += quantity;

            if (savedItem.stack > max)
            {
                int left = savedItem.stack - max;
                savedItem.stack = max;
                AddStackableItem(item, left);
            }
        }
    }
    
    private void AddNotStackableItem(Item item)
    {
        int emptyIndex = GetEmptySlotIndex();
        if (emptyIndex == -1)
        {
            Debug.LogWarning("추가 불가능");
        }
        else
        {
            Items[emptyIndex] = item;
        }
    }

    private int GetEmptySlotIndex()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
                return i;
        }

        return -1;
    }

    public void Sort()
    {
        List<Item> items = Items.Where(item => item != null).ToList();
        
        List<Item> mergedItems = new List<Item>();

        foreach (var item in items)
        {
            var noMax = mergedItems.FirstOrDefault(x => x.Data.id == item.Data.id && x.Data.stackable && x.stack < x.Data.maxStack);

            if (noMax != null)
            {
                int max = item.Data.maxStack;
                int result = noMax.stack + item.stack;

                if (result <= max)
                {
                    noMax.stack = result;
                }
                else
                {
                    noMax.stack = max;

                    Item leftover = item.Clone();
                    leftover.stack = result - max;
                    mergedItems.Add(leftover);
                }
            }
            else
            {
                mergedItems.Add(item.Clone());
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

        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = i < mergedItems.Count ? mergedItems[i] : null;
        }
        
        InventoryUI.RefreshUI(Items);
        QuickSlotInventoryUI.RefreshUI(Items);
    }
}