using UnityEngine;

public class InventoryManager : MonoSingleton<InventoryManager>
{
    public Inventory Inventory { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        Inventory = Helper_Component.FindChildComponent<Inventory>(this.transform, "InventoryContainer");
    }
}