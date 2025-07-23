using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bestRecordText;
    [SerializeField] private TextMeshProUGUI _currentRecordText;

    [SerializeField] private GameObject _slotArea;
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private List<RewardSlot> _slots = new List<RewardSlot>();

    private RectTransform _rect;
    
    private const string BEST_RECORD_TEXT = "BestRecordText";
    private const string CURRENT_RECORD_TEXT = "CurrentRecordText";
    private const string SLOT_AREA = "ClearRewardSlotArea";
    private const string SLOT_PREFAB = "RewardSlot";

    private void Reset()
    {
        _bestRecordText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, BEST_RECORD_TEXT);
        _currentRecordText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, CURRENT_RECORD_TEXT);
        _slotArea = Helper_Component.FindChildGameObjectByName(this.gameObject, SLOT_AREA);
        _slotPrefab = Resources.Load<GameObject>(SLOT_PREFAB);
    }
}
