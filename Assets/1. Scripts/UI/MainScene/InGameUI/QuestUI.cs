using System.Collections;
using TMPro;
using UnityEngine;

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

    /// <summary>
    /// 클리어 시에 텍스트가 깜빡이는 효과<br/>
    /// 튜토리얼 완료 시에는 팝업 UI 활성화
    /// </summary>
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

        if(quest == null)
        {
            TimeManager.Instance.PauseGame(true);
            UIManager.Instance.AskPopUpUI.Open("튜토리얼을 완료하였습니다!\n 바로 게임을 진행하시겠습니까?", OnMainGameStart, OnReturnStartScene);
        }
    }

    /// <summary>
    /// 메인 게임 시작하기
    /// </summary>
    private void OnMainGameStart()
    {
        TimeManager.Instance.PauseGame(false);
        GameManager.Instance.SetTutorial(false);
        if (GlobalState.HasPlayedOpening)
            SaveManager.Instance.LoadGameFromSlot();
        else
            LoadingSceneController.LoadScene("OpeningScene");
    }

    /// <summary>
    /// 시작 화면으로 돌아가기
    /// </summary>
    private void OnReturnStartScene()
    {
        TimeManager.Instance.PauseGame(false);
        GameManager.Instance.SetTutorial(false);
        LoadingSceneController.LoadScene("StartScene");
    }
}
