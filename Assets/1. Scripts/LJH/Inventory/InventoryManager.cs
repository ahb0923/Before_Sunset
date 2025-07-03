using UnityEngine;

public class InventoryManager : MonoSingleton<InventoryManager>
{
    public Inventory Inventory { get; private set; }

    public void Init(Inventory inventory)
    {
        this.Inventory = inventory;
    }
}