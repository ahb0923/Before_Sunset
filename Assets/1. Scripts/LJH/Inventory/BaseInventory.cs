using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInventory : MonoBehaviour
{
    [SerializeField] protected GameObject _inventory;
    [SerializeField] protected GameObject _slotPrefab;
    [SerializeField] protected Transform _itemSlotContainer;
    
    protected List<ItemSlot> _itemSlots = new List<ItemSlot>();

    
    
    protected abstract void InitSlots();

    
    
    public abstract void AddItem(Item item);

    public abstract void RefreshUI();
}