using System.Collections.Generic;

[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;

    public InventorySaveData() 
    {
        items = new List<ItemSaveData>();
    }
}

[System.Serializable]
public class ItemSaveData
{
    public int itemId;
    public int quantity;

    public ItemSaveData(int itemId, int quantity)
    {
        this.itemId = itemId;
        this.quantity = quantity;
    }
}
