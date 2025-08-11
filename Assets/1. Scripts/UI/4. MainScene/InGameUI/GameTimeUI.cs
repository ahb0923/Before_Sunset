using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeUI : MonoBehaviour
{
    [SerializeField] private Image _dayImageUI;
    [SerializeField] private Image _dayPercentageUI;
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private List<Sprite> _dayImages;
    [SerializeField] private List<Sprite> _dayPercentage;
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
            _dayPercentageUI.sprite = _dayPercentage[_dayPercentage.Count - 1];
        }
        else
        {
            for (int i = 1; i <= _dayImages.Count; i++)
            {
                if(IsChangeSprite(i, _dayImages.Count))
                {
                    _dayImageUI.sprite = _dayImages[i - 1];
                    break;
                }
            }

            for (int i = 1; i <= _dayPercentage.Count; i++)
            {
                if (IsChangeSprite(i, _dayPercentage.Count))
                {
                    _dayPercentageUI.sprite = _dayPercentage[i - 1];
                    break;
                }
            }
        }
    }

    private bool IsChangeSprite(int num, int max)
    {
        float flag = (float)num / max;
        return flag > TimeManager.Instance.DailyPercent;
    }

    /// <summary>
    /// 데이 텍스트 설정
    /// </summary>
    public void SetDayText()
    {
        _dayText.text = $"Day {TimeManager.Instance.Day}";
    }
}
