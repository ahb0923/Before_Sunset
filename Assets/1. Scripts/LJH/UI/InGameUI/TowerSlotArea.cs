using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSlotArea : MonoBehaviour
{
    [SerializeField] private List<TowerSlot> _slots = new List<TowerSlot>();
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private GameObject _slotArea;
    [SerializeField] private TowerMaterialSlotArea _materialArea;
    [SerializeField] private Button _buildButton;

    private const string TOWER_SLOT = "TowerSlot";
    private const string TOWER_SLOT_AREA = "TowerSlotArea";
    private const string TOWER_MATERIAL_AREA = "TowerMaterialArea";
    private const string BUILD_BUTTON = "BuildButton";
    
    private void Reset()
    {
        _slotPrefab = Resources.Load<GameObject>(TOWER_SLOT);
        _slotArea = Helper_Component.FindChildByName(this.transform, TOWER_SLOT_AREA).gameObject;
        _materialArea = Helper_Component.FindChildComponent<TowerMaterialSlotArea>(this.transform, TOWER_MATERIAL_AREA);
        _buildButton = Helper_Component.FindChildComponent<Button>(this.transform.parent, BUILD_BUTTON);
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

        _buildButton.onClick.AddListener(Toggle);
    }

    public void ToggleTowerSlotArea()
    {
        if (Input.GetKeyDown(KeyCode.E))
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
    }

    private void Toggle()
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
        _materialArea.Close();
        this.gameObject.SetActive(false);
    }
}
