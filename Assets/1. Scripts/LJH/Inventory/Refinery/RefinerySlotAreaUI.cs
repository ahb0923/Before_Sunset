using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefinerySlotAreaUI : MonoBehaviour
{
    [SerializeField] private GameObject _refinerySlotPrefab;
    [SerializeField] private GameObject _scrollArea;
    
    private const string REFINERY_SLOT = "RefinerySlot";
    private const string SCROLL_AREA = "Content";

    private void Reset()
    {
        _refinerySlotPrefab = Resources.Load<GameObject>(REFINERY_SLOT);
        _scrollArea = UtilityLJH.FindChildInChild(this.transform, SCROLL_AREA).gameObject;
    }

    private void Awake()
    {
        InitSlots();
    }

    private void InitSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            var go = Instantiate(_refinerySlotPrefab, _scrollArea.transform);
            RefinerySlot slot = go.GetComponent<RefinerySlot>();
            slot.SetSlot(i);
        }
    }

    public void AddSlot()
    {
        
    }
}
