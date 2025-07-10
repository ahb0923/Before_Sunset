using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotInventoryUI : MonoBehaviour
{
    [SerializeField] private int _numberOfQuickSlots = 9;
    [SerializeField] private Transform _itemSlotContainer;

    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private PickaxeSlot _pickaxeSlot;
    
    private List<ItemSlot> _quickSlots = new List<ItemSlot>();
    
    private const string ITEM_SLOT_AREA = "QuickSlotArea";
    private const string ITEM_SLOT_PREFAB = "ItemSlot";
    private const string PICKAXE_SLOT = "PickaxeSlot";
    
    
    private void Reset()
    {
        _itemSlotContainer = this.gameObject.transform.Find(ITEM_SLOT_AREA);
        _pickaxeSlot = Helper_Component.FindChildComponent<PickaxeSlot>(this.transform, PICKAXE_SLOT);
        _slotPrefab = Resources.Load<GameObject>(ITEM_SLOT_PREFAB);
    }

    private void Start()
    {
        InitSlots();
        
        if (this.gameObject == null)
        {
            Debug.Log("Quick Slot Inventory is empty");
        }
        else
        {
            if (!this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
        }
        
        _pickaxeSlot.RefreshUI();
        
        foreach (var itemSlot in _quickSlots)
        {
            itemSlot.RefreshUI();
        }
    }

    private void InitSlots()
    {
        for (int i = 0; i < _numberOfQuickSlots; i++)
        {
            var slot = Instantiate(_slotPrefab, _itemSlotContainer);
            _quickSlots.Add(slot.GetComponent<ItemSlot>());
        }
        
        for (int i = 0; i < _quickSlots.Count; i++)
        {
            _quickSlots[i].InitIndex(i);
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

    public void Toggle()
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
    
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void RefreshUI(Item[] items)
    {
        for (int i = 0; i < _quickSlots.Count; i++)
        {
            _quickSlots[i].RefreshUI();
        }
    }
}