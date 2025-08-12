using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartOption : MonoBehaviour
{
    [SerializeField] private Button _exitOptionButton;
    [SerializeField] private RectTransform _optionRect;
    
    [Header("Sound")]
    [SerializeField] private Slider _wholeSoundSlider;
    [SerializeField] private Slider _bGSoundSlider;
    [SerializeField] private Slider _effectSoundSlider;
    [SerializeField] private Button _wholeSoundButton;
    [SerializeField] private Button _bGSoundButton;
    [SerializeField] private Button _effectSoundButton;
    [SerializeField] private GameObject _wholeSoundImage;
    [SerializeField] private GameObject _bGSoundImage;
    [SerializeField] private GameObject _effectSoundImage;
    
    [Header("Window")]
    [SerializeField] private Toggle _windowToggle;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    
    private const string EXIT_OPTION_BUTTON = "ExitOptionButton";
    private const string WHOLE_SOUND_SLIDER = "WholeSoundSlider";
    private const string BG_SOUND_SLIDER = "BGSoundSlider";
    private const string EFFECT_SOUND_SLIDER = "EffectSoundSlider";
    private const string WHOLE_SOUND_BUTTON = "WholeSoundButton";
    private const string WHOLE_SOUND_IMAGE = "WholeSoundButtonImage";
    private const string BG_SOUND_BUTTON = "BGSoundButton";
    private const string BG_SOUND_IMAGE = "BGSoundButtonImage";
    private const string EFFECT_SOUND_BUTTON = "EffectSoundButton";
    private const string EFFECT_SOUND_IMAGE = "EffectSoundButtonImage";
    private const string WINDOW_TOGGLE = "WindowToggle";
    private const string DROPDOWN = "Dropdown";

    private bool _isWholeSoundMute;
    private bool _isBGSoundMute;
    private bool _isEffectSoundMute;
    
    private void Reset()
    {
        _exitOptionButton = Helper_Component.FindChildComponent<Button>(this.transform, EXIT_OPTION_BUTTON);
        _optionRect = GetComponent<RectTransform>();
        _wholeSoundSlider = Helper_Component.FindChildComponent<Slider>(this.transform, WHOLE_SOUND_SLIDER);
        _bGSoundSlider = Helper_Component.FindChildComponent<Slider>(this.transform, BG_SOUND_SLIDER);
        _effectSoundSlider = Helper_Component.FindChildComponent<Slider>(this.transform, EFFECT_SOUND_SLIDER);
        _wholeSoundButton = Helper_Component.FindChildComponent<Button>(this.transform, WHOLE_SOUND_BUTTON);
        _bGSoundButton = Helper_Component.FindChildComponent<Button>(this.transform, BG_SOUND_BUTTON);
        _effectSoundButton = Helper_Component.FindChildComponent<Button>(this.transform, EFFECT_SOUND_BUTTON);
        _wholeSoundImage = Helper_Component.FindChildGameObjectByName(this.gameObject, WHOLE_SOUND_IMAGE);
        _bGSoundImage = Helper_Component.FindChildGameObjectByName(this.gameObject, BG_SOUND_IMAGE);
        _effectSoundImage = Helper_Component.FindChildGameObjectByName(this.gameObject, EFFECT_SOUND_IMAGE);
        _windowToggle = Helper_Component.FindChildComponent<Toggle>(this.transform, WINDOW_TOGGLE);
        _resolutionDropdown = Helper_Component.FindChildComponent<TMP_Dropdown>(this.transform, DROPDOWN);
    }

    private void Awake()
    {
        _exitOptionButton.onClick.AddListener(Close);
        
        _wholeSoundButton.onClick.AddListener(MuteWholeSound);
        _bGSoundButton.onClick.AddListener(MuteBGSound);
        _effectSoundButton.onClick.AddListener(MuteEffectSound);
        
        _windowToggle.isOn = GlobalState.IsFullScreen;
        _windowToggle.onValueChanged.AddListener(SetFullScreen);
        SetFullScreen(GlobalState.IsFullScreen);
    }

    private void Start()
    {
        _wholeSoundSlider.value = AudioManager.Instance.GetWholeVolume();
        _bGSoundSlider.value = AudioManager.Instance.GetBGMVolume();
        _effectSoundSlider.value = AudioManager.Instance.GetSFXVolume();
        
        _wholeSoundSlider.onValueChanged.AddListener(OnWholeVolumeChanged);
        _bGSoundSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        _effectSoundSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        InitResolution();
    }

    public void Open()
    {
        _optionRect.OpenAtCenter();
    }

    private void Close()
    {
        StartSceneManager.Instance.CameraAction();
        _optionRect.CloseAndRestore();
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
    
    private void SetFullScreen(bool isOn)
    {
        Screen.fullScreen = isOn;
        PlayerPrefs.SetInt("IsFullScreen", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void InitResolution()
    {
        _resolutionDropdown.ClearOptions();
        
        var options = new List<string>();

        foreach (var res in GlobalState.Resolutions)
        {
            options.Add($"{res.x} x {res.y}");
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = GlobalState.ResolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        SetResolution(GlobalState.ResolutionIndex);

        _resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }
    
    private void SetResolution(int index)
    {
        var res = GlobalState.Resolutions[index];
        GlobalState.ResolutionIndex = index;
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
    }
}
