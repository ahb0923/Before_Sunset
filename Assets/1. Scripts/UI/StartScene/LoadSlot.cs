using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : MonoBehaviour
{
    [Header("에디터 할당")]
    [SerializeField] private int _slotIndex;
    
    [Header("Reset 할당")]
    [SerializeField] private TextMeshProUGUI _savedStageTxt;
    [SerializeField] private TextMeshProUGUI _savedTimeTxt;
    [SerializeField] private Button _loadButton;

    private void Reset()
    {
        _savedStageTxt = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "SaveStageText");
        _savedTimeTxt = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "SaveTimeText");
        _loadButton = Helper_Component.FindChildComponent<Button>(this.transform, "LoadButton");
    }

    private void Start()
    {
        _loadButton.onClick.AddListener(Load);

        if (SaveManager.Instance.DoesSaveSlotExist(_slotIndex))
        {
            GameData data = SaveManager.Instance.GetGameDataFromSlot(_slotIndex);
            UpdateSavedStageText(data.timeData.day);
            UpdateSavedTimeText(DateTime.Parse(data.lastSaveDateTime));
        }
        else
        {
            _savedStageTxt.text = "";
            _savedTimeTxt.text = "데이터 없음";
        }
    }

    private void Load()
    {
        if (SaveManager.Instance.DoesSaveSlotExist(_slotIndex))
            SaveManager.Instance.LoadGameFromSlot(_slotIndex);
        
        else
            Debug.LogWarning($"{_slotIndex}번 슬롯에는 저장된 데이터가 없습니다!");
    }

    private void UpdateSavedStageText(int day)
    {
        _savedStageTxt.text = $"{day}일 생존 중";
    }

    private void UpdateSavedTimeText(DateTime time)
    {
        _savedTimeTxt.text = time.ToString("HH:mm:ss - yyyy/MM/dd");
    }
}
