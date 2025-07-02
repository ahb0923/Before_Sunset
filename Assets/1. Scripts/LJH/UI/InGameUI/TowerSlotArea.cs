using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlotArea : MonoBehaviour
{
    [SerializeField] private List<TowerSlot> _slots = new List<TowerSlot>();
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private GameObject _slotArea;

    private const string TOWER_SLOT = "TowerSlot";
    private const string TOWER_SLOT_AREA = "TowerSlotArea";
    
    private void Reset()
    {
        _slotPrefab = Resources.Load<GameObject>(TOWER_SLOT);
        _slotArea = UtilityLJH.FindChildInChild(this.transform, TOWER_SLOT_AREA).gameObject;
    }

    private void Awake()
    {
        for (int i = 0; i < 7; i++)
        {
            var slot = Instantiate(_slotPrefab, _slotArea.transform);
            var towerSlot = slot.GetComponent<TowerSlot>();
            _slots.Add(towerSlot);
            towerSlot.InitIndex(i);
            //towerSlot.SetSlot();
        }
    }

    public void ToggleTowerSlotArea()
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

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
}
