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
            //UpdateSavedTimeText(data.timeData.stage, data.timeData.day, data.timeData.dailyTime > TimeManager.Instance.DailyPercent);
        }
    }

    private void Save()
    {
        UpdateSavedStageText(TimeManager.Instance.Stage, TimeManager.Instance.Day, TimeManager.Instance.IsNight);
        UpdateSavedTimeText();
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

    private void UpdateSavedTimeText()
    {
        DateTime now = DateTime.Now;
        savedTimeTxt.text = now.ToString("HH:mm:ss - yyyy/MM/dd");
    }
}
