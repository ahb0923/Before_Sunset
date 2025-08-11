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
    [SerializeField] private Image _fadeImage;
    
    [Header("버튼")]
    [SerializeField] private RectTransform _buttonContainer;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _loadGameButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TextMeshProUGUI _newGameText;
    [SerializeField] private TextMeshProUGUI _loadGameText;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private TextMeshProUGUI _optionText;
    [SerializeField] private TextMeshProUGUI _exitGameText;
    [SerializeField] private Image _newGameButtonImage;
    [SerializeField] private Image _loadGameButtonImage;
    [SerializeField] private Image _tutorialButtonImage;
    [SerializeField] private Image _optionButtonImage;
    [SerializeField] private Image _exitButtonImage;
    
    [Header("Awake시 할당")]
    public SelectImage selectImage;
    public AskTutorial askTutorial;
    public Load load;
    public StartOption startOption;
    
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
        _fadeImage = Helper_Component.FindChildComponent<Image>(this.transform, "FadeImage");
        _buttonContainer = Helper_Component.FindChildComponent<RectTransform>(this.transform, "ButtonContainer");
        _newGameButton = Helper_Component.FindChildComponent<Button>(this.transform, "NewGameButton");
        _loadGameButton = Helper_Component.FindChildComponent<Button>(this.transform, "LoadGameButton");
        _tutorialButton = Helper_Component.FindChildComponent<Button>(this.transform, "TutorialButton");
        _optionButton = Helper_Component.FindChildComponent<Button>(this.transform, "OptionButton");
        _exitButton = Helper_Component.FindChildComponent<Button>(this.transform, "ExitGameButton");
        _newGameButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "NewGameButton");
        _loadGameButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "LoadGameButton");
        _tutorialButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "TutorialButton");
        _optionButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "OptionButton");
        _exitButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, "ExitGameButton");
        _newGameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "NewGameButtonText");
        _loadGameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "LoadGameButtonText");
        _tutorialText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "TutorialButtonText");
        _optionText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "OptionButtonText");
        _exitGameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, "ExitGameButtonText");
    }

    private void Awake()
    {
        selectImage = Helper_Component.FindChildComponent<SelectImage>(this.transform, "SelectContainer");
        askTutorial = Helper_Component.FindChildComponent<AskTutorial>(this.transform, "AskTutorial");
        load = Helper_Component.FindChildComponent<Load>(this.transform, "Load");
        startOption = Helper_Component.FindChildComponent<StartOption>(this.transform, "Option");
        _newGameButton.onClick.AddListener(NewGame);
        _loadGameButton.onClick.AddListener(LoadGame);
        _tutorialButton.onClick.AddListener(Tutorial);
        _optionButton.onClick.AddListener(Option);
        _exitButton.onClick.AddListener(Exit);
    }
    
    private void Start()
    {
        if (GlobalState.HasPlayedIntro)
            Open();
        else
            RaycastTarget(false);
    }

    public void RaycastTarget(bool isTrue)
    {
        _newGameButtonImage.raycastTarget = isTrue;
        _loadGameButtonImage.raycastTarget = isTrue;
        _tutorialButtonImage.raycastTarget = isTrue;
        _optionButtonImage.raycastTarget = isTrue;
        _exitButtonImage.raycastTarget = isTrue;
        
        _newGameText.raycastTarget = isTrue;
        _loadGameText.raycastTarget = isTrue;
        _tutorialText.raycastTarget = isTrue;
        _optionText.raycastTarget = isTrue;
        _exitGameText.raycastTarget = isTrue;
    }

    public void Open()
    {
        _backRect.OpenAtCenter();
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
        logoSequence.InsertCallback(1.5f, PlayBGM);
        // 버튼 페이드인
        logoSequence.Insert(2.5f,_buttonContainer.DOAnchorPos(new Vector2(600f, 0), _duration));
        logoSequence.Insert(2.5f,_newGameText.DOFade(1f, _duration));
        logoSequence.Insert(2.5f,_loadGameText.DOFade(1f, _duration));
        logoSequence.Insert(2.5f,_tutorialText.DOFade(1f, _duration));
        logoSequence.Insert(2.5f,_optionText.DOFade(1f, _duration));
        logoSequence.Insert(2.5f,_exitGameText.DOFade(1f, _duration));
        logoSequence.AppendInterval(0.5f);
        // 배경 페이드아웃
        logoSequence.Append(_background.DOFade(0.9372549f, _duration)).
            OnComplete(() =>
            {
                logoSequence.Kill();
                EnableButtons();
                RaycastTarget(true);
            });
        
        GlobalState.HasPlayedIntro = true;
    }

    private void PlayBGM()
    {
        AudioManager.Instance.PlayBGM("Main");
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
        _optionText.alpha = 0f;
        _exitGameText.alpha = 0f;
    }

    private void EnableButtons()
    {
        _newGameButton.interactable = true;
        _loadGameButton.interactable = true;
        _tutorialButton.interactable = true;
        _optionButton.interactable = true;
        _exitButton.interactable = true;
    }

    private void DisableButtons()
    {
        _newGameButton.interactable = false;
        _loadGameButton.interactable = false;
        _tutorialButton.interactable = false;
        _optionButton.interactable = false;
        _exitButton.interactable = false;
    }

    private void NewGame()
    {
        StartSceneManager.Instance.StopCamera();
        selectImage.HideImage();
        askTutorial.Open(false);
    }

    private void LoadGame()
    {
        StartSceneManager.Instance.StopCamera();
        selectImage.HideImage();
        load.Open();
    }

    private void Tutorial()
    {
        StartSceneManager.Instance.StopCamera();
        selectImage.HideImage();
        askTutorial.Open(true);
    }

    private void Option()
    {
        StartSceneManager.Instance.StopCamera();
        selectImage.HideImage();
        startOption.Open();
    }

    private void Exit()
    {
#if UNITY_EDITOR
        StartSceneManager.Instance.StopCamera();
        selectImage.HideImage();
        UnityEditor.EditorApplication.isPlaying = false;
#else
        StartSceneManager.Instance.StopCamera();
        selectImage.HideImage();
        Application.Quit();
#endif
    }

    public void FadeOut()
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.DOFade(0f, 2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            RaycastTarget(true);
            _fadeImage.gameObject.SetActive(false);
        });
    }
}
