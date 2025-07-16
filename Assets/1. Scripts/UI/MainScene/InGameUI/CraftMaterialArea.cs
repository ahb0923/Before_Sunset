using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftMaterialArea : MonoBehaviour
{
    [SerializeField] private List<BuildingMaterialSlot> _materialSlots = new List<BuildingMaterialSlot>();
    [SerializeField] private GameObject _materialSlotPrefab;
    [SerializeField] private GameObject _materialSlotArea;

    private const string BUILDING_MATERIAL_SLOT = "Slots/BuildingMaterialSlot";
    private const string BUILDING_MATERIAL_SLOT_AREA = "BuildingMaterialSlotArea";
    
    private void Reset()
    {
        _materialSlotPrefab = Resources.Load<GameObject>(BUILDING_MATERIAL_SLOT);
        _materialSlotArea = Helper_Component.FindChildByName(this.transform, BUILDING_MATERIAL_SLOT_AREA).gameObject;
    }

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            var slot = Instantiate(_materialSlotPrefab, _materialSlotArea.transform);
            var materialSlot = slot.GetComponent<BuildingMaterialSlot>();
            _materialSlots.Add(materialSlot);
            materialSlot.InitIndex(i);
            slot.SetActive(false);
        }
    }
    
    public void SetMaterialSlot(TowerDatabase data)
    {
        List<KeyValuePair<string, int>> dataList = data.buildRequirements.ToList();
        List<Item> items = InventoryManager.Instance.Inventory.Items.ToList();
        
        foreach (var slot in _materialSlots)
        {
            slot.ClearSlot();

            if (slot.Index < dataList.Count)
            {
                var dataName = dataList[slot.Index].Key;
                var dataAmount = dataList[slot.Index].Value;
                
                slot.SetSlot(dataName, dataAmount, items);
            }
        }
    }
    
    public void SetMaterialSlot(SmelterDatabase data)
    {
        List<KeyValuePair<string, int>> dataList = data.buildRequirements.ToList();
        List<Item> items = InventoryManager.Instance.Inventory.Items.ToList();
    
        foreach (var slot in _materialSlots)
        {
            slot.ClearSlot();

            if (slot.Index < dataList.Count)
            {
                var dataName = dataList[slot.Index].Key;
                var dataAmount = dataList[slot.Index].Value;
            
                slot.SetSlot(dataName, dataAmount, items);
            }
        }
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
