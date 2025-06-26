using UnityEngine;

public class InventoryManager : MonoSingleton<InventoryManager>
{
    public Inventory Inventory { get; private set; }
    
    public ItemData itemData;
    public ItemData itemData2;
    public ItemData itemData3;

    public void Init(Inventory inventory)
    {
        this.Inventory = inventory;
    }
}