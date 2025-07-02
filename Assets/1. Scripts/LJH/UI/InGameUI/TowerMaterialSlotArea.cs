using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerMaterialSlotArea : MonoBehaviour
{
    [SerializeField] private List<TowerMaterialSlot> _materialSlots = new List<TowerMaterialSlot>();
    [SerializeField] private GameObject _materialSlotPrefab;
    [SerializeField] private GameObject _materialSlotArea;

    private const string TOWER_MATERIAL_SLOT = "TowerMaterialSlot";
    private const string TOWER_MATERIAL_SLOT_AREA = "TowerMaterialSlotArea";
    
    private void Reset()
    {
        _materialSlotPrefab = Resources.Load<GameObject>(TOWER_MATERIAL_SLOT);
        _materialSlotArea = UtilityLJH.FindChildInChild(this.transform, TOWER_MATERIAL_SLOT_AREA).gameObject;
    }

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            var slot = Instantiate(_materialSlotPrefab, _materialSlotArea.transform);
            var materialSlot = slot.GetComponent<TowerMaterialSlot>();
            _materialSlots.Add(materialSlot);
            materialSlot.InitIndex(i);
            slot.SetActive(false);
        }

        var go = this.gameObject.transform.parent.gameObject;
        go.SetActive(false);
        Close();
    }

    public void SetMaterialSlot(TowerData data)
    {
        foreach (var slot in _materialSlots)
        {
            slot.ClearSlot();
        }
    }

    public void ToggleMaterialSlotArea()
    {
        bool isOpen = this.gameObject.activeSelf;
        this.gameObject.SetActive(!isOpen);

        if (!isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        foreach (var slot in _materialSlots)
        {
            slot.ClearSlot();
        }
        
        this.gameObject.SetActive(false);
    }
}
