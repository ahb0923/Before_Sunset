using UnityEngine;
using UnityEngine.UI;

public class TutorialSkip : MonoBehaviour
{
    private Button tutoSkipBtn;

    private void Awake()
    {
        tutoSkipBtn = GetComponent<Button>();
        tutoSkipBtn.onClick.AddListener(OnClickTutorialSkipButton);
    }

    /// <summary>
    /// 스킵 버튼 클릭 시에 일시 정지 후 팝업 UI 활성화
    /// </summary>
    private void OnClickTutorialSkipButton()
    {
        TimeManager.Instance.PauseGame(true);
        UIManager.Instance.AskPopUpUI.Open("튜토리얼을 스킵하고 게임을 시작하시겠습니까?", () => OnSkip(), () => TimeManager.Instance.PauseGame(false));
    }

    /// <summary>
    /// 튜토리얼 스킵 시에 메인 게임 시작
    /// </summary>
    private void OnSkip()
    {
        TimeManager.Instance.PauseGame(false);
        GameManager.Instance.SetTutorial(false);
        GlobalState.Index = 1;
        LoadingSceneController.LoadScene("MainScene");
    }
}
