using UnityEngine;
using UnityEngine.UI;

public class AskTutorial : MonoBehaviour
{
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private RectTransform _askRect;
    
    private const string YES_BUTTON = "YesButton";
    private const string NO_BUTTON = "NoButton";

    private void Reset()
    {
        _yesButton = Helper_Component.FindChildComponent<Button>(this.transform, YES_BUTTON);
        _noButton = Helper_Component.FindChildComponent<Button>(this.transform, NO_BUTTON);
        _askRect = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        _yesButton.onClick.AddListener(Tutorial);
        _noButton.onClick.AddListener(NoTutorial);
    }

    public void Open()
    {
        _askRect.OpenAtCenter();
    }

    private void Tutorial()
    {
        Close();
        GameManager.Instance.SetTutorial(true);
        LoadingSceneController.LoadScene("TutorialScene");
    }

    private void NoTutorial()
    {
        Close();
        GameManager.Instance.SetTutorial(false);
        if (GlobalState.HasPlayedOpening)
            LoadingSceneController.LoadScene("MainScene");
        else
            LoadingSceneController.LoadScene("OpeningScene");
    }
    
    private void Close()
    {
        _askRect.CloseAndRestore();
    }
}
