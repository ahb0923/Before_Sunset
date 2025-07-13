using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartSceneButton : MonoBehaviour
{
    [SerializeField] private Button _singleButton;
    [SerializeField] private Button _multiButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _quitButton;

    [SerializeField] private Image _musicButtonImage;
    // [SerializeField] private AudioSource _musicBox;
    
    private const string SINGLE_BUTTON = "SingleButton";
    private const string MULTI_BUTTON = "MultiButton";
    private const string MUSIC_BUTTON = "MusicButton";
    private const string QUIT_BUTTON = "QuitButton";
    
    private void Reset()
    {
        _singleButton = Helper_Component.FindChildComponent<Button>(this.transform, SINGLE_BUTTON);
        _multiButton = Helper_Component.FindChildComponent<Button>(this.transform, MULTI_BUTTON);
        _musicButton = Helper_Component.FindChildComponent<Button>(this.transform, MUSIC_BUTTON);
        _quitButton = Helper_Component.FindChildComponent<Button>(this.transform, QUIT_BUTTON);
        
        _musicButtonImage = Helper_Component.FindChildComponent<Image>(this.transform, MUSIC_BUTTON);
        // _musicBox = GameObject.Find("MusicBox").GetComponent<AudioSource>();
    }

    private void Start()
    {
        // _musicBox.Play();
        
        _singleButton.onClick.AddListener(OnClickSingleButton);
        _multiButton.onClick.AddListener(OnClickMultiButton);
        _musicButton.onClick.AddListener(OnClickMusicButton);
        _quitButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickSingleButton()
    {
        LoadingSceneController.LoadScene("MainScene");
    }

    private void OnClickMultiButton()
    {
        LoadingSceneController.LoadScene("MainScene");
    }

    private void OnClickMusicButton()
    {
        // if (!_musicBox.isPlaying)
        // {
        //     _musicBox.Play();
        //     
        //     _musicButtonImage.DOColor(Color.white, 0.3f);
        // }
        // else
        // {
        //     _musicBox.Stop();
        //     
        //     _musicButtonImage.DOColor(Color.red, 0.3f);
        // }
    }

    private void OnClickQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
