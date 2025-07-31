using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeUI : MonoBehaviour
{
    [Header("# Related Time")]
    [SerializeField] private Transform _dayPieceParent;
    private Transform[] _dayPieceList;
    [SerializeField] private TextMeshProUGUI _stageText;
    [SerializeField] private Transform _clockHand;

    [Header("# Buttons")]
    [SerializeField] private Button _halfDaySkipBtn;
    [SerializeField] private Button _daySkipBtn;

    private void Awake()
    {
        _dayPieceList = _dayPieceParent.GetComponentsInChildren<Transform>();

        _halfDaySkipBtn.onClick.AddListener(TimeManager.Instance.SkipHalfDay);
        _daySkipBtn.onClick.AddListener(TimeManager.Instance.NextDay);
    }

    private void Update()
    {
        UpdateClockHand();
        UpdateSkipButtonState();
    }

    /// <summary>
    /// 밤낮 시계 바늘의 회전 업데이트
    /// </summary>
    private void UpdateClockHand()
    {
        _clockHand.rotation = Quaternion.Euler(0, 0, -TimeManager.Instance.DailyPercent * 360);
    }

    /// <summary>
    /// 몬스터 소환 상태에 따른 스킵 버튼 활성화/비활성화 업데이트
    /// </summary>
    private void UpdateSkipButtonState()
    {
        // 몬스터 소환 전(1 ~ 3일차)에는 활성화
        if(TimeManager.Instance.Day <= TimeManager.Instance.MaxDay - 2)
        {
            _daySkipBtn.interactable = true;
            _halfDaySkipBtn.interactable = true;
        }
        else
        {
            // 몬스터 소환 전(4일차 낮)에는 반나절 스킵만 활성화
            if (TimeManager.Instance.Day == TimeManager.Instance.MaxDay - 1 && !TimeManager.Instance.IsNight)
            {
                _halfDaySkipBtn.interactable = true;
                _daySkipBtn.interactable = false;
            }
            else
            {
                // 몬스터 소환 후에는 클리어 상태에 따라서 활성화 또는 비활성화
                if (TimeManager.Instance.IsStageClear)
                {
                    _halfDaySkipBtn.interactable = true;
                    _daySkipBtn.interactable = true;
                }
                else
                {
                    _halfDaySkipBtn.interactable = false;
                    _daySkipBtn.interactable = false;
                }
            }
        }
    }

    /// <summary>
    /// 날짜에 따른 날짜 피스 UI 설정
    /// </summary>
    public void SetDayPieces()
    {
        for (int i = 1; i < _dayPieceList.Length; i++) 
        {
            _dayPieceList[i].gameObject.SetActive(i <= TimeManager.Instance.Day);
        }
    }

    /// <summary>
    /// 스테이지 텍스트 설정
    /// </summary>
    public void SetStageText()
    {
        _stageText.text = $"{TimeManager.Instance.Stage} 주차";
    }
}
