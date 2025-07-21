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
    /// �ð� �ٴ� ���ư����� ������Ʈ
    /// </summary>
    private void UpdateClockHand()
    {
        _clockHand.rotation = Quaternion.Euler(0, 0, -TimeManager.Instance.DailyPercent * 360);
    }

    /// <summary>
    /// ��¥�� �ð��� ���� ��ŵ ��ư ��ȣ�ۿ� ���� ���� ������Ʈ
    /// </summary>
    private void UpdateSkipButtonState()
    {
        // �Ͻ� ���� �ÿ��� ��Ȱ��ȭ
        if (TimeManager.Instance.IsGamePause)
        {
            _daySkipBtn.interactable = false;
            _halfDaySkipBtn.interactable = false;
        }

        // 3���� ������ Ȱ��ȭ
        if(TimeManager.Instance.Day <= TimeManager.Instance.MaxDay - 2)
        {
            _daySkipBtn.interactable = true;
            _halfDaySkipBtn.interactable = true;
        }
        else
        {
            // 4���� ������ ���� ���� ��ŵ�� ����
            if (TimeManager.Instance.Day == TimeManager.Instance.MaxDay - 1 && !TimeManager.Instance.IsNight)
            {
                _halfDaySkipBtn.interactable = true;
                _daySkipBtn.interactable = false;
            }
            else
            {
                // ���� ���� ���Ŀ��� ��� ���Ͱ� �׾��� ���� ��ŵ ����
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
    /// ��¥�� ���� ��¥ �ǽ� Ȱ��ȭ
    /// </summary>
    public void SetDayPieces()
    {
        for (int i = 1; i < _dayPieceList.Length; i++) 
        {
            _dayPieceList[i].gameObject.SetActive(i <= TimeManager.Instance.Day);
        }
    }

    /// <summary>
    /// �������� �ؽ�Ʈ ������Ʈ
    /// </summary>
    public void SetStageText()
    {
        _stageText.text = $"{TimeManager.Instance.Stage} Stage";
    }
}
