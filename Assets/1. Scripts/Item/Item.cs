using System;
using UnityEngine;

public class Item
{
    public ItemDatabase Data { get; private set; }

    public bool Stackable => (Data.id >= 100 && Data.id <= 399);
    public int stack;
    public int MaxStack => Stackable ? 99 : 0;
    public bool IsMaxStack => stack >= MaxStack;

    // public int quantity;

    public Item(ItemDatabase data) => Data = data;

    public Item Clone()
    {
        var clone = new Item(this.Data);
        clone.stack = this.stack;
        return clone;
    }
}
