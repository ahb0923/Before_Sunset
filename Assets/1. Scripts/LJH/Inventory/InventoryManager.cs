using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public MainInventory mainInventory;
    public QuickSlotInventory quickSlotInventory;
    
    public ItemData itemData;
    public ItemData itemData2;

    //싱글톤 패턴 구현 전이라 임시로 달아놓음
    //InventoryUI에서 업데이트 될거임
    private void Update()
    {
        mainInventory.ToggleInventory();
        quickSlotInventory.ToggleInventory();

        if (Input.GetKeyDown(KeyCode.E))
        {
            mainInventory.AddItem(itemData.CreateItem());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            mainInventory.AddItem(itemData2.CreateItem());
        }
    }
}