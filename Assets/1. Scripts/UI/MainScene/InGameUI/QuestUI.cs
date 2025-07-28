using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _questTitle;
    [SerializeField] private TextMeshProUGUI _questDesc;
    [SerializeField] private Image _clearImg;
    [SerializeField] private Image _doingImg;

    /// <summary>
    /// 새 퀘스트에 관한 UI 업데이트
    /// </summary>
    public void UpdateQuestUI(Quest quest)
    {
        if(quest == null)
        {
            _questTitle.text = "모든 퀘스트 완료!";
            _questDesc.text = "";
            _clearImg.gameObject.SetActive(true);
            _doingImg.gameObject.SetActive(false);
            return;
        }

        _questTitle.text = quest.Title;
        _questDesc.text = quest.Description;
        _clearImg.gameObject.SetActive(false);
        _doingImg.gameObject.SetActive(true);
    }
}
