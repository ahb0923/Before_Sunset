using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _questTitle;
    [SerializeField] private TextMeshProUGUI _questDesc;
    [SerializeField] private TextMeshProUGUI _questAmount;

    [SerializeField] private float _duration = 0.2f;

    /// <summary>
    /// 새 퀘스트에 관한 UI 업데이트
    /// </summary>
    public void UpdateQuestUI(Quest quest)
    {
        if(quest == null)
        {
            _questTitle.text = "모든 퀘스트 완료!";
            _questDesc.text = "";
            _questAmount.text = "(1/1)";
            return;
        }

        _questTitle.text = quest.Title;
        _questDesc.text = quest.Description;
        _questAmount.text = "(" + quest.CurAmount + "/" + quest.ClearAmount + ")";
    }

    public void DisplayClear(Quest quest)
    {
        StartCoroutine(C_DisplayClear(quest));
    }

    private IEnumerator C_DisplayClear(Quest quest)
    {
        float timer = _duration;
        while (timer >= 0f)
        {
            _questTitle.color = _questTitle.color.WithAlpha(timer / _duration);
            _questDesc.color = _questDesc.color.WithAlpha(timer / _duration);
            _questAmount.color = _questAmount.color.WithAlpha(timer / _duration);

            timer -= Time.deltaTime;
            yield return null;
        }

        UpdateQuestUI(quest);

        timer = 0f;
        while (timer <= _duration)
        {
            _questTitle.color = _questTitle.color.WithAlpha(timer / _duration);
            _questDesc.color = _questDesc.color.WithAlpha(timer / _duration);
            _questAmount.color = _questAmount.color.WithAlpha(timer / _duration);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
