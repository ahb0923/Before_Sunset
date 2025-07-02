using System;
using UnityEngine;

public class Item
{
    public ItemData Data { get; private set; }

    public bool Stackable => (Data.id >= 100 && Data.id <= 399);
    public int stack;
    public int MaxStack => Stackable ? 20 : 0;
    public bool IsMaxStack => stack >= MaxStack;

    public int quantity;

    public Item(ItemData data) => Data = data;

    public Item Clone()
    {
        var clone = new Item(this.Data);
        clone.stack = this.stack;
        return clone;
    }
}
