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
        _saveBtn.onClick.AddListener(() => UIManager.Instance.AskPopUpUI.Open($"{slotIndex}번 슬롯에 데이터를 저장하시겠습니까?", onYesAction: Save));
        _loadBtn.onClick.AddListener(() => UIManager.Instance.AskPopUpUI.Open($"{slotIndex}번 슬롯에 데이터를 불러오시겠습니까?", onYesAction: Load));

        if (SaveManager.Instance.DoesSaveSlotExist(slotIndex))
        {
            GameData data = SaveManager.Instance.GetGameDataFromSlot(slotIndex);
        }
    }

    private void Save()
    {
        UpdateSavedStageText(TimeManager.Instance.Stage, TimeManager.Instance.Day, TimeManager.Instance.IsNight);
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
}
