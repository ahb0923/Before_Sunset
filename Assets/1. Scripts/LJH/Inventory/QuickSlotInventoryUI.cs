using System.Collections.Generic;
using UnityEngine;

public class QuickSlotInventoryUI : MonoBehaviour
{
    [SerializeField] private int _numberOfQuickSlots = 9;
    [SerializeField] private Transform _itemSlotContainer;
    
    [SerializeField] private GameObject _slotPrefab;
    
    private List<ItemSlot> _quickSlots = new List<ItemSlot>();
    
    private const string ITEM_SLOT_AREA = "QuickSlotArea";
    private const string ITEM_SLOT_PREFAB = "ItemSlot";
    
    
    private void Reset()
    {
        _itemSlotContainer = this.gameObject.transform.Find(ITEM_SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(ITEM_SLOT_PREFAB);
    }

    private void Start()
    {
        //InventoryManager.Instance.Init(this);
        
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
    }

    private void InitSlots()
    {
        for (int i = 0; i < _numberOfQuickSlots; i++)
        {
            var slot = Instantiate(_slotPrefab, _itemSlotContainer);
            _quickSlots.Add(slot.GetComponent<ItemSlot>());
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

    public void RefreshUI()
    {
        foreach (var slot in _quickSlots)
        {
            slot.RefreshUI();
        }
    }
}