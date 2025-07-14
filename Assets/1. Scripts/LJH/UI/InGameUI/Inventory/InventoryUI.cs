using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private int _numberOfQuickSlots = 9;
    [SerializeField] private int _numberOfItemSlots = 20;
    [SerializeField] private Transform _itemSlotContainer;
    [SerializeField] private Transform _quickSlotContainer;
    
    
    [SerializeField] private PickaxeSlot _pickaxeSlot;
    [SerializeField] private GameObject _slotPrefab;
    
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    private const string QUICK_SLOT_AREA = "QuickSlotArea";
    private const string ITEM_SLOT_AREA = "ItemSlotArea";
    private const string ITEM_SLOT_PREFAB = "Slots/ItemSlot";
    private const string PICKAXE_SLOT = "PickaxeSlot";
    
    
    private void Reset()
    {
        _quickSlotContainer = this.gameObject.transform.Find(QUICK_SLOT_AREA);
        _itemSlotContainer = this.gameObject.transform.Find(ITEM_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(ITEM_SLOT_PREFAB);
        _pickaxeSlot = Helper_Component.FindChildComponent<PickaxeSlot>(this.transform, PICKAXE_SLOT);
    }
    
    private void Start()
    {
        InitSlots();
        
        _pickaxeSlot.RefreshUI();
        
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.RefreshUI();
        }

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
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

    public void Toggle()
    {
        bool isActiveSelf = this.gameObject.activeSelf;
        this.gameObject.SetActive(!isActiveSelf);
    }
    
    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
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

    public void RefreshPickaxe()
    {
        _pickaxeSlot.RefreshUI();
    }
}