using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInventory : MonoBehaviour
{
    [SerializeField] protected GameObject _inventory;
    [SerializeField] protected GameObject _slotPrefab;
    [SerializeField] protected Transform _itemSlotContainer;
    
    protected List<ItemSlot> _itemSlots = new List<ItemSlot>();

    protected virtual void Awake()
    {
        if (_inventory == null)
        {
            Debug.Log("Inventory is empty");
        }
        else
        {
            if (_inventory.activeSelf)
            {
                _inventory.SetActive(false);
            }
        }

        InitSlots();

        foreach (var itemSlot in _itemSlots)
        {
            itemSlot.UpdateUI();
        }
    }
    
    protected abstract void InitSlots();

    public virtual void ToggleInventory()
    {
        if (_inventory != null)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool isOpen = _inventory.activeSelf;
                _inventory.SetActive(!isOpen);

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
    }
    
    protected virtual void Open()
    {
        _inventory.SetActive(true);
    }

    protected virtual void Close()
    {
        _inventory.SetActive(false);
    }
    
    public abstract void AddItem(Item item);

    public abstract void RefreshUI();
}