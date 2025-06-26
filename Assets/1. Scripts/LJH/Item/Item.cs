using System;
using UnityEngine;

public class Item
{
    //테스트용
    public ItemData Data { get; private set; }
    
    public int stack;
    public bool IsMaxStack => stack >= Data.maxStack;

    public Item(ItemData data) => Data = data;

    public Item Clone()
    {
        var clone = new Item(this.Data);
        clone.stack = this.stack;
        return clone;
    }
}