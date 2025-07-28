using UnityEngine;
using UnityEngine.UI;

public class StartSceneButton : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _startButton.onClick.AddListener(GameManager.Instance.StartNewGame);
        _loadButton.onClick.AddListener(GameManager.Instance.StartSavedGame);
        _tutorialButton.onClick.AddListener(GameManager.Instance.StartTutorial);
        _quitButton.onClick.AddListener(GameManager.Instance.QuitGame);
    }
}
