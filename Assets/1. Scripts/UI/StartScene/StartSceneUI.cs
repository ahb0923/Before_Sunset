using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

public class StartSceneUI : MonoBehaviour
{
    [Header("UI이미지")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _logoImage;
    [SerializeField] private RectTransform _backRect;
    [SerializeField] private RectTransform _logoRect;
    
    [Header("버튼")]
    [SerializeField] private RectTransform _buttonContainer;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _loadGameButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TextMeshProUGUI _newGameText;
    [SerializeField] private TextMeshProUGUI _loadGameText;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private TextMeshProUGUI _exitGameText;
    [SerializeField] private Image _newGameButtonImage;
    [SerializeField] private Image _loadGameButtonImage;
    [SerializeField] private Image _tutorialButtonImage;
    [SerializeField] private Image _exitButtonImage;
    
    [Header("Awake시 할당")]
    public SelectImage selectImage;
    public AskTutorial askTutorial;
    public Load load;
    
    private float _duration = 1f;
    private float _duration2 = 2f;
    private Vector3 _originalPos = new Vector3(-260f, 0, 0);
    private Vector3 _originalRot = Vector3.zero;

    private void Reset()
    {
        _background = Helper_Component.FindChildComponent<Image>(this.transform, "Background");
        _logoImage = Helper_Component.FindChildComponent<Image>(this.transform, "LogoImage");
        _backRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, "Background");
        _logoRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, "LogoImage");
        _buttonContainer = Helper_Component.FindChildComponent<RectTransform>(this.transform, "ButtonContainer");
        _newGameButton = Helper_Component.FindChildComponent<Button>(this.transform, "NewGameButton");
        _loadGameButton = Helper_Component.FindChildComponent<Button>(this.transform, "LoadGameButton");
        _tutorialButton = Helper_Component.FindChildComponent<Button>(this.transform, "TutorialButton");
        _exitButton = Helper_Component.FindChildComponent<Button>(this.transform, "ExitGameButton");
        _newGameButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "NewGameButton");
        _loadGameButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "LoadGameButton");
        _tutorialButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "TutorialButton");
        _exitButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "ExitGameButton");
        _newGameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "NewGameButtonText");
        _loadGameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "LoadGameButtonText");
        _tutorialText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "TutorialButtonText");
        _exitGameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "ExitGameButtonText");
    }

    private void Awake()
    {
        selectImage = Helper_Component.FindChildComponent<SelectImage>(this.transform, "SelectContainer");
        askTutorial = Helper_Component.FindChildComponent<AskTutorial>(this.transform, "AskTutorial");
        load = Helper_Component.FindChildComponent<Load>(this.transform, "Load");
        _newGameButton.onClick.AddListener(NewGame);
        _loadGameButton.onClick.AddListener(LoadGame);
        _tutorialButton.onClick.AddListener(Tutorial);
        _exitButton.onClick.AddListener(Exit);
        
        RaycastTarget(false);
    }

    private void RaycastTarget(bool isTrue)
    {
        _newGameButtonImage.raycastTarget = isTrue;
        _loadGameButtonImage.raycastTarget = isTrue;
        _tutorialButtonImage.raycastTarget = isTrue;
        _exitButtonImage.raycastTarget = isTrue;
        
        _newGameText.raycastTarget = isTrue;
        _loadGameText.raycastTarget = isTrue;
        _tutorialText.raycastTarget = isTrue;
        _exitGameText.raycastTarget = isTrue;
    }
    
    private void Start()
    {
        if (GlobalState.HasPlayedIntro)
        {
            _backRect.OpenAtCenter();
        }
    }

    [Button]
    public void TitleAnimation()
    {
        _backRect.OpenAtCenter();
        ResetTitle();
        ResetButtons();
        DisableButtons();
        Sequence logoSequence = DOTween.Sequence();
        // 로고 팝업
        logoSequence.Append(_logoRect.DOScale(Vector3.one, _duration).SetEase(Ease.OutBounce));
        logoSequence.Join(_logoImage.DOFade(1, _duration));
        logoSequence.AppendInterval(0.5f);
        // 로고 굴리기
        logoSequence.Append(_logoRect.DORotate(_originalRot, _duration2));
        logoSequence.Join(_logoRect.DOAnchorPos(_originalPos, _duration2));
        // 버튼 페이드인
        logoSequence.Insert(2.5f,_buttonContainer.DOAnchorPos(new Vector2(600f, 0), _duration));
        logoSequence.Insert(2.5f,_newGameText.DOFade(1f, _duration));
        logoSequence.Insert(2.5f,_loadGameText.DOFade(1f, _duration));
        logoSequence.Insert(2.5f,_tutorialText.DOFade(1f, _duration));
        logoSequence.Insert(2.5f,_exitGameText.DOFade(1f, _duration));
        logoSequence.AppendInterval(0.5f);
        // 배경 페이드아웃
        logoSequence.Append(_background.DOFade(0.8f, _duration)).
            OnComplete(() =>
            {
                logoSequence.Kill();
                EnableButtons();
                RaycastTarget(true);
                AudioManager.Instance.PlayBGM("Main");
            });
        
        GlobalState.HasPlayedIntro = true;
    }

    private void ResetTitle()
    {
        _logoImage.color = new Color(1, 1, 1, 0);
        _logoRect.anchoredPosition = Vector3.zero;
        _logoRect.rotation = Quaternion.Euler(0, 0, -60f);
        _logoRect.localScale = Vector3.zero;
        _background.color = Color.black;
    }

    private void ResetButtons()
    {
        _buttonContainer.anchoredPosition = new Vector2(500, 0);
        _newGameText.alpha = 0f;
        _loadGameText.alpha = 0f;
        _tutorialText.alpha = 0f;
        _exitGameText.alpha = 0f;
    }

    private void EnableButtons()
    {
        _newGameButton.interactable = true;
        _loadGameButton.interactable = true;
        _tutorialButton.interactable = true;
        _exitButton.interactable = true;
    }

    private void DisableButtons()
    {
        _newGameButton.interactable = false;
        _loadGameButton.interactable = false;
        _tutorialButton.interactable = false;
        _exitButton.interactable = false;
    }

    private void NewGame()
    {
        StartSceneManager.Instance.StartSceneAnimation.StopCamera();
        selectImage.HideImage();
        askTutorial.Open(false);
    }

    private void LoadGame()
    {
        StartSceneManager.Instance.StartSceneAnimation.StopCamera();
        selectImage.HideImage();
        load.Open();
    }

    private void Tutorial()
    {
        StartSceneManager.Instance.StartSceneAnimation.StopCamera();
        selectImage.HideImage();
        askTutorial.Open(true);
    }

    private void Exit()
    {
#if UNITY_EDITOR
        StartSceneManager.Instance.StartSceneAnimation.StopCamera();
        selectImage.HideImage();
        UnityEditor.EditorApplication.isPlaying = false;
#else
        StartSceneManager.Instance.StartSceneAnimation.StopCamera();
        selectImage.HideImage();
        Application.Quit();
#endif
    }
}
