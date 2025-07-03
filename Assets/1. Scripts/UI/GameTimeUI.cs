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
    [SerializeField] private Button _pauseBtn;
    [SerializeField] private Button _halfDaySkipBtn;
    [SerializeField] private Button _daySkipBtn;

    private void Awake()
    {
        _dayPieceList = _dayPieceParent.GetComponentsInChildren<Transform>();

        _pauseBtn.onClick.AddListener(TimeManager.Instance.ControlPause);
        _halfDaySkipBtn.onClick.AddListener(TimeManager.Instance.SkipHalfDay);
        _daySkipBtn.onClick.AddListener(TimeManager.Instance.NextDay);
    }

    private void Update()
    {
        UpdateClockHand();
        UpdateSkipButtonState();
    }

    /// <summary>
    /// 시계 바늘 돌아가도록 업데이트
    /// </summary>
    private void UpdateClockHand()
    {
        _clockHand.rotation = Quaternion.Euler(0, 0, -TimeManager.Instance.DailyPercent * 360);
    }

    /// <summary>
    /// 날짜와 시간에 따른 스킵 버튼 상호작용 가능 유무 업데이트
    /// </summary>
    private void UpdateSkipButtonState()
    {
        if(TimeManager.Instance.Day <= TimeManager.Instance.MaxDay - 2)
        {
            _daySkipBtn.interactable = true;
            _halfDaySkipBtn.interactable = true;
        }
        else
        {
            if (TimeManager.Instance.Day == TimeManager.Instance.MaxDay - 1 && !TimeManager.Instance.IsNight)
                _halfDaySkipBtn.interactable = true;
            else
                _halfDaySkipBtn.interactable = false;

            _daySkipBtn.interactable = false;
        }
    }

    /// <summary>
    /// 날짜에 따라서 날짜 피스 활성화
    /// </summary>
    public void SetDayPieces(int day)
    {
        for (int i = 1; i < _dayPieceList.Length; i++) 
        {
            _dayPieceList[i].gameObject.SetActive(i <= day);
        }
    }

    /// <summary>
    /// 스테이지 텍스트 업데이트
    /// </summary>
    public void SetStageText(int stage)
    {
        _stageText.text = $"{stage} Stage";
    }
}
