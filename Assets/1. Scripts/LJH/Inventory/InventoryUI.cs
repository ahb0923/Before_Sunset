using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private int _numberOfQuickSlots = 9;
    [SerializeField] private int _numberOfItemSlots = 20;
    [SerializeField] private Transform _itemSlotContainer;
    [SerializeField] private Transform _quickSlotContainer;
    [SerializeField] private Button _sortButton;
    
    [SerializeField] private GameObject _slotPrefab;
    
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    private const string QUICK_SLOT_AREA = "QuickSlotArea";
    private const string ITEM_SLOT_AREA = "ItemSlotArea";
    private const string ITEM_SLOT_PREFAB = "ItemSlot";
    private const string SORT_BUTTON = "SortButton";
    
    private void Reset()
    {
        _quickSlotContainer = this.gameObject.transform.Find(QUICK_SLOT_AREA);
        _itemSlotContainer = this.gameObject.transform.Find(ITEM_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(ITEM_SLOT_PREFAB);
        _sortButton = UtilityLJH.FindChildComponent<Button>(this.transform, SORT_BUTTON);
    }
    
    private void Start()
    {
        // InventoryManager.Instance.Init(this);
        
        if (this.gameObject == null)
        {
            Debug.Log("Inventory is empty");
        }
        else
        {
            if (this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }
        
        InitSlots();
        
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.RefreshUI();
        }
    }
    
    private void InitSlots()
    {
        for (int i = 0; i < _numberOfQuickSlots; i++)
        {
            var slot = Instantiate(_slotPrefab, _quickSlotContainer);
            itemSlots.Add(slot.GetComponent<ItemSlot>());
        }
        
        for (int i = 0; i < _numberOfItemSlots; i++)
        {
            var slot = Instantiate(_slotPrefab, _itemSlotContainer);
            itemSlots.Add(slot.GetComponent<ItemSlot>());
        }
        
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].InitIndex(i);
        }
    }
    
    
    /// <summary>
    /// 인벤토리UI 활성화 / 비활성화 스위치 메서드
    /// </summary>
    public void ToggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            bool isOpen = this.gameObject.activeSelf;
            this.gameObject.SetActive(!isOpen);

            if (!isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
    }
    
    private void Open()
    {
        this.gameObject.SetActive(true);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 인벤토리 아이템 슬롯 새로고침 메서드
    /// </summary>
    public void RefreshUI(Item[] items)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].RefreshUI();
        }
    }
}