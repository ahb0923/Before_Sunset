using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Item Data", fileName = "New Item Data")]
public class ItemData : ScriptableObject
{
    //테스트용
    public string itemName;
    public Sprite icon;
    public int quantity;
    
    public int maxStack = 20;
    public bool stackable;
    
    public Item CreateItem()
    {
        return new Item(this);
    }
}