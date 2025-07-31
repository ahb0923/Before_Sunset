using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour, ICloseableUI
{
    [SerializeField] private Slider _wholeSoundSlider;
    [SerializeField] private Slider _bGSoundSlider;
    [SerializeField] private Slider _effectSoundSlider;
    [SerializeField] private Button _cancelButton;
    
    private RectTransform _optionRect;
    
    private string _quitText = "마지막 저장은 자동 저장된 내용입니다.\n저장하지 않고 종료하시겠습니까?";
    private string _mainMenuText = "마지막 저장은 자동 저장된 내용입니다.\n저장하지 않고 메인메뉴로 나가시겠습니까?";
    
    private const string WHOLE_SOUND_SLIDER = "WholeSoundSlider";
    private const string BG_SOUND_SLIDER = "BGSoundSlider";
    private const string EFFECT_SOUND_SLIDER = "EffectSoundSlider";
    
    private const string OPTION_BUTTON = "OptionButton";
    private const string UN_PAUSE_BUTTON = "UnPauseButton";
    private const string QUIT_GAME_BUTTON = "QuitGameButton";
    private const string SAVE_LOAD_OPTION_BUTTON = "SaveLoadOptionButton";
    private const string MAIN_MENU_OPTION_BUTTON = "MainMenuOptionButton";
    private const string CANCEL_BUTTON = "BackGroundCancelButton";

    private void Reset()
    {
        _wholeSoundSlider = Helper_Component.FindChildComponent<Slider>(this.transform, WHOLE_SOUND_SLIDER);
        _bGSoundSlider = Helper_Component.FindChildComponent<Slider>(this.transform, BG_SOUND_SLIDER);
        _effectSoundSlider = Helper_Component.FindChildComponent<Slider>(this.transform, EFFECT_SOUND_SLIDER);
        _cancelButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_BUTTON);
    }

    private void Awake()
    {
        Button optionButton = Helper_Component.FindChildComponent<Button>(this.transform.parent, OPTION_BUTTON);
        Button unPauseButton = Helper_Component.FindChildComponent<Button>(this.transform, UN_PAUSE_BUTTON);
        Button quitGameButton = Helper_Component.FindChildComponent<Button>(this.transform, QUIT_GAME_BUTTON);
        Button mainMenuButton = Helper_Component.FindChildComponent<Button>(this.transform, MAIN_MENU_OPTION_BUTTON);
        Button saveLoadButton = Helper_Component.FindChildComponent<Button>(this.transform, SAVE_LOAD_OPTION_BUTTON);
        
        optionButton.onClick.AddListener(Open);
        unPauseButton.onClick.AddListener(Close);
        quitGameButton.onClick.AddListener(OnClickQuitButton);
        saveLoadButton.onClick.AddListener(OnClickSaveLoadButton);
        mainMenuButton.onClick.AddListener(OnClickMainMenuButton);
        _cancelButton.onClick.AddListener(Close);
        
        _optionRect = this.GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        // _wholeSoundSlider.value = AudioManager.Instance().Volume;
        // _bGSoundSlider.value = AudioManager.Instance().BgmVolume;
        // _effectSoundSlider.value = AudioManager.Instance().SfxVolume;
    }

    private void Update()
    {
        // AudioManager.Instance().SetVolume(_wholeSoundSlider.value);
        // AudioManager.Instance().SetBGMVolume(_bGSoundSlider.value);
        // AudioManager.Instance().SetSFXVolume(_effectSoundSlider.value);
    }

    public void Open()
    {
        UIManager.Instance.OpenUI(this);
    }

    public void Close()
    {
        UIManager.Instance.CloseUI(this);
    }

    public void OpenUI()
    {
        _optionRect.OpenAtCenter();
        TimeManager.Instance.PauseGame(true);
    }

    public void CloseUI()
    {
        _optionRect.CloseAndRestore();
        TimeManager.Instance.PauseGame(false);
    }

    private void OnClickSaveLoadButton()
    {
        if (GameManager.Instance.IsTutorial)
        {
            ToastManager.Instance.ShowToast("튜토리얼 중에는 세이브를 할 수 없습니다!");
            return;
        }

        if (DefenseManager.Instance.MonsterSpawner.IsMonsterAlive)
        {
            ToastManager.Instance.ShowToast("몬스터 디펜스 중에는 세이브를 할 수 없습니다!");
            return;
        }

        UIManager.Instance.SaveLoadUI.Open();
    }

    private void OnClickMainMenuButton()
    {
        var popUpUI = UIManager.Instance.AskPopUpUI;
        popUpUI.Open(_mainMenuText, MainMenu, null);
    }

    private void OnClickQuitButton()
    {
        var popUpUI = UIManager.Instance.AskPopUpUI;
        popUpUI.Open(_quitText, Quit, null);
    }

    private void MainMenu()
    {
        TimeManager.Instance.PauseGame(false);
        SceneManager.LoadScene("StartScene");
    }
    
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
