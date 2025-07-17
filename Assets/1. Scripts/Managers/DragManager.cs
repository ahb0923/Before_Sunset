using UnityEngine;

public class DragManager : PlainSingleton<DragManager>
{
    public static Item DraggingItem;
    public static GameObject DraggingIcon;
    public static ItemSlot OriginItemSlot;
    public static SmelterSlot OriginSmelterSlot;

    public static void Clear()
    {
        DraggingItem = null;
        OriginItemSlot = null;
        OriginSmelterSlot = null;
    }
}
