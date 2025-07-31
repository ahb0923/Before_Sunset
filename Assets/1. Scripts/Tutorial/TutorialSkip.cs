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

    private void OnClickTutorialSkipButton()
    {
        TimeManager.Instance.PauseGame(true);
        UIManager.Instance.AskPopUpUI.Open("튜토리얼을 스킵하시겠습니까?", () => OnSkip(), () => TimeManager.Instance.PauseGame(false));
    }

    private void OnSkip()
    {
        TimeManager.Instance.PauseGame(false);
        GameManager.Instance.SetTutorial(false);
        LoadingSceneController.LoadScene("MainScene");
    }
}
