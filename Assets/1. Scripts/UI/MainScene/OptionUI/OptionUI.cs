using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour, ICloseableUI
{
    [SerializeField] private Slider _wholeSoundSlider;
    [SerializeField] private Slider _bGSoundSlider;
    [SerializeField] private Slider _effectSoundSlider;
    [SerializeField] private Button _wholeSoundButton;
    [SerializeField] private Button _bGSoundButton;
    [SerializeField] private Button _effectSoundButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private GameObject _wholeSoundImage;
    [SerializeField] private GameObject _bGSoundImage;
    [SerializeField] private GameObject _effectSoundImage;
    
    private RectTransform _optionRect;
    private bool _isWholeSoundMute;
    private bool _isBGSoundMute;
    private bool _isEffectSoundMute;
    
    private string _quitText = "마지막 저장은 자동 저장된 내용입니다.\n저장하지 않고 종료하시겠습니까?";
    private string _mainMenuText = "마지막 저장은 자동 저장된 내용입니다.\n저장하지 않고 메인메뉴로 나가시겠습니까?";
    
    private const string WHOLE_SOUND_SLIDER = "WholeSoundSlider";
    private const string BG_SOUND_SLIDER = "BGSoundSlider";
    private const string EFFECT_SOUND_SLIDER = "EffectSoundSlider";
    private const string WHOLE_SOUND_BUTTON = "WholeSoundButton";
    private const string WHOLE_SOUND_IMAGE = "WholeSoundButtonImage";
    private const string BG_SOUND_BUTTON = "BGSoundButton";
    private const string BG_SOUND_IMAGE = "BGSoundButtonImage";
    private const string EFFECT_SOUND_BUTTON = "EffectSoundButton";
    private const string EFFECT_SOUND_IMAGE = "EffectSoundButtonImage";
    
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
        _wholeSoundButton = Helper_Component.FindChildComponent<Button>(this.transform, WHOLE_SOUND_BUTTON);
        _bGSoundButton = Helper_Component.FindChildComponent<Button>(this.transform, BG_SOUND_BUTTON);
        _effectSoundButton = Helper_Component.FindChildComponent<Button>(this.transform, EFFECT_SOUND_BUTTON);
        _cancelButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_BUTTON);
        _wholeSoundImage = Helper_Component.FindChildGameObjectByName(this.gameObject, WHOLE_SOUND_IMAGE);
        _bGSoundImage = Helper_Component.FindChildGameObjectByName(this.gameObject, BG_SOUND_IMAGE);
        _effectSoundImage = Helper_Component.FindChildGameObjectByName(this.gameObject, EFFECT_SOUND_IMAGE);
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
        _wholeSoundButton.onClick.AddListener(MuteWholeSound);
        _bGSoundButton.onClick.AddListener(MuteBGSound);
        _effectSoundButton.onClick.AddListener(MuteEffectSound);
        _cancelButton.onClick.AddListener(Close);
        
        _optionRect = this.GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        _wholeSoundSlider.value = AudioManager.Instance.GetWholeVolume();
        _bGSoundSlider.value = AudioManager.Instance.GetBGMVolume();
        _effectSoundSlider.value = AudioManager.Instance.GetSFXVolume();
        
        AudioManager.Instance.SetWholeVolume(_wholeSoundSlider.value);
        AudioManager.Instance.SetBGMVolume(_bGSoundSlider.value);
        AudioManager.Instance.SetSFXVolume(_effectSoundSlider.value);
        
        _wholeSoundSlider.onValueChanged.AddListener(OnWholeVolumeChanged);
        _bGSoundSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        _effectSoundSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        CloseUI();
    }

    public void Open()
    {
        UIManager.Instance.OpenUIClosingEveryUI(this);
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

    private void MuteWholeSound()
    {
        if (_isWholeSoundMute)
        {
            _isWholeSoundMute = false;
            _wholeSoundImage.gameObject.SetActive(true);
        }
        else
        {
            _isWholeSoundMute = true;
            _wholeSoundImage.gameObject.SetActive(false);
        }
        AudioManager.Instance.SetWholeMute(_isWholeSoundMute);
    }

    private void MuteBGSound()
    {
        if (_isBGSoundMute)
        {
            _isBGSoundMute = false;
            _bGSoundImage.gameObject.SetActive(true);
        }
        else
        {
            _isBGSoundMute = true;
            _bGSoundImage.gameObject.SetActive(false);
        }
        AudioManager.Instance.SetBGMMute(_isBGSoundMute);
    }

    private void MuteEffectSound()
    {
        if (_isEffectSoundMute)
        {
            _isEffectSoundMute = false;
            _effectSoundImage.gameObject.SetActive(true);
        }
        else
        {
            _isEffectSoundMute = true;
            _effectSoundImage.gameObject.SetActive(false);
        }
        AudioManager.Instance.SetSFXMute(_isEffectSoundMute);
    }

    private void OnClickSaveLoadButton()
    {
        if (GameManager.Instance.IsTutorial)
        {
            ToastManager.Instance.ShowToast("튜토리얼 중에는 세이브를 할 수 없습니다!");
            return;
        }

        if (TimeManager.Instance.IsNight)
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

    private void OnWholeVolumeChanged(float value)
    {
        AudioManager.Instance.SetWholeVolume(value);
    }

    private void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}
