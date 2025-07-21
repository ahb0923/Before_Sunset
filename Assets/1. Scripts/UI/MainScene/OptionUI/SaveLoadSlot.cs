using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadSlot : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private TextMeshProUGUI savedStageTxt;
    [SerializeField] private TextMeshProUGUI savedTimeTxt;
    [SerializeField] private Button _saveBtn;
    [SerializeField] private Button _loadBtn;

    private void Awake()
    {
        _saveBtn.onClick.AddListener(() => Save());
        _loadBtn.onClick.AddListener(() => Load());
    }

    private void Start()
    {
        if (SaveManager.Instance.DoesSaveSlotExist(slotIndex))
        {
            GameData data = SaveManager.Instance.GetGameDataFromSlot(slotIndex);
            UpdateSavedStageText(data.timeData.stage, data.timeData.day, data.timeData.isNight);
            UpdateSavedTimeText(DateTime.Parse(data.lastSaveDateTime));
        }
    }

    private void Save()
    {
        UpdateSavedStageText(TimeManager.Instance.Stage, TimeManager.Instance.Day, TimeManager.Instance.IsNight);
        UpdateSavedTimeText(DateTime.Now);
        SaveManager.Instance.SaveGameToSlot(slotIndex);
    }

    private void Load()
    {
        if (SaveManager.Instance.DoesSaveSlotExist(slotIndex))
            SaveManager.Instance.LoadGameFromSlot(slotIndex);
        else
            Debug.LogWarning($"{slotIndex}번 슬롯에는 저장된 데이터가 없습니다!");
    }

    private void UpdateSavedStageText(int stage, int day, bool isNight)
    {
        string DayOrNight = isNight ? "밤" : "낮";
        savedStageTxt.text = $"{stage}주째 {day}일의 " + DayOrNight;
    }

    private void UpdateSavedTimeText(DateTime time)
    {
        savedTimeTxt.text = time.ToString("HH:mm:ss - yyyy/MM/dd");
    }
}
