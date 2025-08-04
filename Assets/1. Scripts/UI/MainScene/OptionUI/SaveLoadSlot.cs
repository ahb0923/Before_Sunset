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

    [SerializeField] private bool _isAutoSave = false;

    private void Start()
    {
        if(_saveBtn != null)
        {
            _saveBtn.onClick.AddListener(() => UIManager.Instance.AskPopUpUI.Open($"{slotIndex}번 슬롯에 데이터를 저장하시겠습니까?", onYesAction: Save));
        }
        
        if(_loadBtn != null)
        {
            _loadBtn.onClick.AddListener(OnClickLoad);
        }

        if (SaveManager.Instance.DoesSaveSlotExist(slotIndex))
        {
            GameData data = SaveManager.Instance.GetGameDataFromSlot(slotIndex);
            UpdateSavedStageText(data.timeData.day);
            UpdateSavedTimeText(DateTime.Parse(data.lastSaveDateTime));
        }
        else
        {
            savedStageTxt.text = "";
            savedTimeTxt.text = _isAutoSave ? "저장된 데이터가 없음!" : "데이터 저장 가능!";
        }
    }

    /// <summary>
    /// 세이브 팝업 UI에 Yes 버튼 클릭에 의한 저장
    /// </summary>
    public void Save()
    {
        UpdateSavedStageText(TimeManager.Instance.Day);
        UpdateSavedTimeText(DateTime.Now);
        SaveManager.Instance.SaveGameToSlot(slotIndex);
    }

    /// <summary>
    /// 로드 버튼 클릭 시, 실행
    /// </summary>
    private void OnClickLoad()
    {
        if (SaveManager.Instance.DoesSaveSlotExist(slotIndex))
        {
            if (slotIndex != 99)
                UIManager.Instance.AskPopUpUI.Open($"{slotIndex}번 슬롯에 데이터를 불러오시겠습니까?", onYesAction: Load);
            else
                UIManager.Instance.AskPopUpUI.Open($"자동 슬롯에 데이터를 불러오시겠습니까?", onYesAction: Load);
        }
        else
            ToastManager.Instance.ShowToast("저장된 데이터가 없습니다!");
    }

    /// <summary>
    /// 로드 팝업 UI에 Yes 버튼 클릭으로 인한 로드
    /// </summary>
    private void Load()
    {
        SaveManager.Instance.LoadGameFromSlot(slotIndex);
        UIManager.Instance.CloseEveryUI();
    }

    /// <summary>
    /// 생존 일수 텍스트 업데이트
    /// </summary>
    private void UpdateSavedStageText(int day)
    {
        savedStageTxt.text = $"{day}일 생존 중";
    }

    /// <summary>
    /// 저장 시간 텍스트 업데이트
    /// </summary>
    private void UpdateSavedTimeText(DateTime time)
    {
        savedTimeTxt.text = time.ToString("HH:mm:ss - yyyy/MM/dd");
    }
}
