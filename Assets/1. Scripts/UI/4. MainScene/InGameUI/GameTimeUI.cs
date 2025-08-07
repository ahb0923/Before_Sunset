using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeUI : MonoBehaviour
{
    [SerializeField] private Image _dayImageUI;
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private List<Sprite> _dayImages;
    [SerializeField] private Sprite _nightImage;

    private void Update()
    {
        UpdateDayImage();
    }

    /// <summary>
    /// 시간 경과에 따른 날짜 이미지 변경
    /// </summary>
    private void UpdateDayImage()
    {
        if (TimeManager.Instance.IsNight)
        {
            _dayImageUI.sprite = _nightImage;
        }
        else
        {
            for (int i = 1; i <= _dayImages.Count; i++)
            {
                float flag = (float) i / _dayImages.Count;
                if(flag > TimeManager.Instance.DailyPercent)
                {
                    _dayImageUI.sprite = _dayImages[i - 1];
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 데이 텍스트 설정
    /// </summary>
    public void SetDayText()
    {
        _dayText.text = $"Day {TimeManager.Instance.Day}";
    }
}
