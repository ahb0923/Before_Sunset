using UnityEngine;
using UnityEngine.UI;

public class Load : MonoBehaviour
{
    [SerializeField] private Button _exitLoadButton;
    [SerializeField] private RectTransform _loadRect;
    [SerializeField] private StartSceneAnimation _startSceneAnimation;
    
    private const string EXIT_LOAD_BUTTON = "ExitLoadButton";

    private void Reset()
    {
        _exitLoadButton = Helper_Component.FindChildComponent<Button>(this.transform, EXIT_LOAD_BUTTON);
        _loadRect = GetComponent<RectTransform>();
        _startSceneAnimation = FindObjectOfType<StartSceneAnimation>();
    }

    private void Awake()
    {
        _exitLoadButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        _loadRect.OpenAtCenter();
    }

    private void Close()
    {
        _startSceneAnimation.CameraAction();
        _loadRect.CloseAndRestore();
    }
}
