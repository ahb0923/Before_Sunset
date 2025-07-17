using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoSingleton<InventoryManager>
{
    public Inventory Inventory { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        Inventory = Helper_Component.FindChildComponent<Inventory>(this.transform, "InventoryContainer");
    }
    
    public bool IsEnoughMaterial(TowerDatabase data)
    {
        List<KeyValuePair<string, int>> dataList = data.buildRequirements.ToList();
        List<Item> items = Inventory.Items.ToList();

        for (int i = 0; i < dataList.Count; i++)
        {
            var dataName = dataList[i].Key;
            var requiredAmount = dataList[i].Value;
            var amount = CountMaterial(dataName, items);

            if (amount > requiredAmount)
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    
    public int CountMaterial(string dataName, List<Item> items)
    {
        int count = 0;

        foreach (var item in items)
        {
            if (item == null)
            {
                continue;
            }
            
            if (item.Data.itemName == dataName)
            {
                count += item.stack;
            }
        }
        
        return count;
    }
}