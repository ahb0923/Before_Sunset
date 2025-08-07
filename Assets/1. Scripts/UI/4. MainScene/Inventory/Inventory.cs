using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, ISaveable, ICloseableUI
{
    public Item Pickaxe { get; private set; }
    public Item[] Items { get; private set; } = new Item[29];

    [field: SerializeField] public InventoryUI InventoryUI { get; private set; }
    [field: SerializeField] public QuickSlotInventoryUI QuickSlotInventoryUI { get; private set; }
    [SerializeField] private Button _inventoryButton;
    private HashSet<int> _addedSlotIndex = new HashSet<int>();
    public bool IsInventoryFull { get; private set; } = false;

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
        Button sortButton = Helper_Component.FindChildComponent<Button>(this.transform, SORT_BUTTON);
        sortButton.onClick.AddListener(Sort);
        _inventoryButton.onClick.AddListener(Toggle);
        SetPickaxe(DataManager.Instance.EquipmentData.GetById(700));
    }

    /// <summary>
    /// 인벤토리와 퀵슬롯 토글 메서드
    /// </summary>
    public void Toggle()
    {
        if (InventoryUI.gameObject.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }

        if (TooltipManager.Instance != null)
            TooltipManager.Instance.HideTooltip();
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
        InventoryUI.gameObject.SetActive(true);
        QuickSlotInventoryUI.gameObject.SetActive(false);

        foreach (var slot in QuickSlotInventoryUI.quickSlots)
        {
            slot.DisableHighlight();
        }
    }

    public void CloseUI()
    {
        InventoryUI.gameObject.SetActive(false);
        QuickSlotInventoryUI.gameObject.SetActive(true);
        TooltipManager.Instance.HideTooltip();

        foreach (var slot in InventoryUI.itemSlots)
        {
            slot.DisableHighlight();
        }
    }

    /// <summary>
    /// 곡괭이 초기화 메서드
    /// </summary>
    /// <param name="data"></param>
    public void SetPickaxe(EquipmentDatabase data)
    {
        Pickaxe = null;
        Pickaxe = new Item(data);
    }

    /// <summary>
    /// 아이템의 id 값으로 아이템 만드는 메서드
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private Item CreateItem(int id)
    {
        Item item = new Item(DataManager.Instance.ItemData.GetById(id));
        return item;
    }

    /// <summary>
    /// 인벤토리UI 새로고침 메서드
    /// </summary>
    public void RefreshInventories()
    {
        InventoryUI.RefreshUI(Items);
        QuickSlotInventoryUI.RefreshUI(Items);
    }

    public void RefreshPickaxeSlots()
    {
        InventoryUI.RefreshPickaxe();
        QuickSlotInventoryUI.RefreshPickaxe();
    }

    /// <summary>
    /// 인벤토리에 아이템 추가 메서드
    /// </summary>
    /// <param name="id">추가 할 아이템</param>
    /// <param name="quantity">아이템 수량</param>
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
        foreach (var index in _addedSlotIndex)
        {
            if (index < QuickSlotInventoryUI.quickSlots.Count)
            {
                InventoryUI.itemSlots[index].AddAnimation();
                QuickSlotInventoryUI.quickSlots[index].AddAnimation();
            }
            else
            {
                InventoryUI.itemSlots[index].AddAnimation();
            }
        }
        _addedSlotIndex.Clear();
        QuestManager.Instance?.AddQuestAmount(QUEST_TYPE.GainItem, id, quantity);

        var slots = UIManager.Instance.CraftArea.buildSlots;
        foreach (var slot in slots)
        {
            slot.CheckBuildable();
        }
    }

    /// <summary>
    /// 겹칠 수 있는 아이템을 인벤토리에 추가하는 메서드
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity"></param>
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
                _addedSlotIndex.Add(i);
                break;
            }
        }

        if (savedItem == null)
        {
            int emptyIndex = GetEmptySlotIndex();
            if (emptyIndex == -1)
            {
                ToastManager.Instance.ShowToast("인벤토리가 가득 찼습니다.");
                IsInventoryFull = true;
            }
            else
            {
                item.stack += quantity;
                Items[emptyIndex] = item;
                _addedSlotIndex.Add(emptyIndex);
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

    /// <summary>
    /// 겹칠 수 없는 아이템을 인벤토리에 추가하는 메서드
    /// </summary>
    /// <param name="item"></param>
    private void AddNotStackableItem(Item item)
    {
        int emptyIndex = GetEmptySlotIndex();
        if (emptyIndex == -1)
        {
            ToastManager.Instance.ShowToast("인벤토리가 가득 찼습니다.");
            IsInventoryFull = true;
        }
        else
        {
            Items[emptyIndex] = item;
            _addedSlotIndex.Add(emptyIndex);
        }
    }

    /// <summary>
    /// 인벤토리에서 첫번째 빈 슬롯을 찾아주는 메서드
    /// </summary>
    /// <returns></returns>
    private int GetEmptySlotIndex()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 아이템 정렬 메서드
    /// </summary>
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

        ToastManager.Instance.ShowToast("정렬 완료");
    }

    /// <summary>
    /// 『효빈』아이템 갯수 체크 메서드 
    /// </summary>
    /// <param name="itemName">아이템 이름</param>
    /// <returns>갯수값 리턴</returns>
    public int GetItemCount(string itemName)
    {
        int count = 0;
        foreach (var item in Items)
        {
            if (item != null && item.Data.itemName == itemName)
            {
                count += item.stack;
            }
        }
        return count;
    }

    /// <summary>
    /// 아이템 사용 메서드
    /// </summary>
    /// <param name="id">사용할 아이템의 아이디</param>
    /// <param name="quantity">사용할 아이템의 수량</param>
    /// <returns>사용하면 T 불가능하면 F</returns>
    public bool UseItem(int id, int quantity)
    {
        if (quantity <= 0)
        {
            return false;
        }

        int total = 0;

        foreach (Item item in Items)
        {
            if (item != null && item.Data.id == id)
            {
                total += item.stack;
            }
        }

        if (total < quantity)
        {
            return false;
        }

        int remain = quantity;

        for (int i = 0; i < Items.Length; i++)
        {
            Item item = Items[i];

            if (item == null || item.Data.id != id)
            {
                continue;
            }

            if (item.stack > remain)
            {
                item.stack -= remain;
                break;
            }
            else
            {
                remain -= item.stack;
                Items[i] = null;
            }
        }

        InventoryUI.RefreshUI(Items);
        QuickSlotInventoryUI.RefreshUI(Items);
        
        var slots = UIManager.Instance.CraftArea.buildSlots;
        foreach (var slot in slots)
        {
            slot.CheckBuildable();
        }
        
        return true;
    }



    /// <summary>
    /// 아이템 사용 메서드
    /// </summary>
    /// <param name="itemName">사용할 아이템의 이름</param>
    /// <param name="quantity">사용할 아이템의 수량</param>
    /// <returns>사용하면 T 불가능하면 F</returns>
    public bool UseItem(string itemName, int quantity)
    {
        if (quantity <= 0)
        {
            return false;
        }

        int total = 0;

        foreach (Item item in Items)
        {
            if (item != null && item.Data.itemName == itemName)
            {
                total += item.stack;
            }
        }

        if (total < quantity)
        {
            return false;
        }

        int remain = quantity;

        for (int i = 0; i < Items.Length; i++)
        {
            Item item = Items[i];

            if (item == null || item.Data.itemName != itemName)
            {
                continue;
            }

            if (item.stack > remain)
            {
                item.stack -= remain;
                break;
            }
            else
            {
                remain -= item.stack;
                Items[i] = null;
            }
        }

        InventoryUI.RefreshUI(Items);
        QuickSlotInventoryUI.RefreshUI(Items);
        
        var slots = UIManager.Instance.CraftArea.buildSlots;
        foreach (var slot in slots)
        {
            slot.CheckBuildable();
        }
        
        return true;
    }
    
    /// <summary>
    /// 인벤토리 데이터 저장
    /// </summary>
    public void SaveData(GameData data)
    {
        InventorySaveData invenData = data.inventory;

        invenData.pickaxe = new ItemSaveData(Pickaxe.Data.id, 1);

        foreach(Item item in Items)
        {
            ItemSaveData itemData;
            if(item != null)
            {
                itemData = new ItemSaveData(item.Data.id, item.stack == 0 ? 1 : item.stack);
            }
            else
            {
                itemData = new ItemSaveData(-1, 0);
            }

            invenData.items.Add(itemData);
        }
    }

    /// <summary>
    /// 인벤토리 데이터 로드
    /// </summary>
    public void LoadData(GameData data)
    {
        InventorySaveData invenData = data.inventory;

        SetPickaxe(DataManager.Instance.EquipmentData.GetById(invenData.pickaxe.itemId));

        for (int i = 0; i < Items.Length; i++)
        {
            if (invenData.items[i].itemId == -1)
            {
                Items[i] = null;
            }
            else
            {
                Items[i] = CreateItem(invenData.items[i].itemId);
                Items[i].stack = invenData.items[i].quantity;
            }
        }

        RefreshInventories();
    }
}