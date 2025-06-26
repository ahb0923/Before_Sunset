using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;

    public Inventory Inventory { get; private set; }
    public InventoryUI InventoryUI { get; private set; }
    public QuickSlotInventoryUI QuickSlotInventoryUI { get; private set; }

    public static InventoryManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<InventoryManager>();
            if (_instance == null)
            {
                GameObject go = new GameObject("InventoryManager");
                _instance = go.AddComponent<InventoryManager>();
            }
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public void Init(Inventory inventory)
    {
        Inventory = inventory;
    }

    // public void Init(QuickSlotInventoryUI quickSlotInventoryUI)
    // {
    //     QuickSlotInventoryUI = quickSlotInventoryUI;
    // }

    // public void Init(InventoryUI inventoryUI)
    // {
    //     InventoryUI = inventoryUI;
    // }
    
    public ItemData itemData;
    public ItemData itemData2;
    public ItemData itemData3;
}