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
    [SerializeField] private Button _inventoryButton;
    
    private const string SORT_BUTTON = "SortButton";
    private const string INVENTORY_BUTTON = "InventoryButton";

    private void Reset()
    {
        InventoryUI = GetComponentInChildren<InventoryUI>();
        QuickSlotInventoryUI = GetComponentInChildren<QuickSlotInventoryUI>();
        _inventoryButton = Helper_Component.FindChildComponent<Button>(this.transform.parent.parent, INVENTORY_BUTTON);
    }

    private void Awake()
    {
        Button sortButton = UtilityLJH.FindChildComponent<Button>(this.transform, SORT_BUTTON);
        sortButton.onClick.AddListener(Sort);
        _inventoryButton.onClick.AddListener(Toggle);
        //Pickaxe
    }

    private void Update()
    {
        InventoryUI.ToggleInventory();
        QuickSlotInventoryUI.ToggleInventory();
    }

    private void Toggle()
    {
        InventoryUI.Toggle();
        QuickSlotInventoryUI.Toggle();
    }

    public Item CreateItem(int id)
    {
        Item item = new Item(DataManager.Instance.ItemData.GetId(id));
        return item;
    }
    
    /// <summary>
    /// 인벤토리에 아이템 추가 메서드
    /// </summary>
    /// <param name="item">추가 할 아이템</param>
    public void AddItem(int id, int quantity)
    {
        Item item = CreateItem(id);
        
        if (item.Stackable)
        {
            AddStackableItem(item, quantity);
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
            int max = savedItem.MaxStack;
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
            var noMax = mergedItems.FirstOrDefault(x => x.Data.id == item.Data.id && x.Stackable && x.stack < x.MaxStack);

            if (noMax != null)
            {
                int max = item.MaxStack;
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
            
            bool isAMax = a.Stackable && a.stack == a.MaxStack;
            bool isBMax = b.Stackable && b.stack == b.MaxStack;
            
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