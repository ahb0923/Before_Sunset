using System;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class InGame : MonoBehaviour
{
    [SerializeField] private TowerSlotArea _towerSlotArea;

    private void Reset()
    {
        _towerSlotArea = GetComponentInChildren<TowerSlotArea>();
    }

    private void Update()
    {
        _towerSlotArea.ToggleTowerSlotArea();
    }
}