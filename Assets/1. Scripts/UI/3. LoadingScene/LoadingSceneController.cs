using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    private static string NEXT_SCENE;
    
    [SerializeField] private Image _loadingBar;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private TextMeshProUGUI _loadingPercent;
    //[SerializeField] private Animator _loadingAnimator;
    [SerializeField] private string[] _anims =
    {
        "LoadingGoblin",
        "LoadingWolf",
        "LoadingBee",
    };
    [SerializeField] private TextMeshProUGUI _tipText;
    [SerializeField] private string[] _tips =
    {
        "팁: (당근을 흔드는 중입니다.)",
        // "팁: UI개발자를 제외한 인원들은 가짜 스탠다드!",
        "팁: 이쁘게 봐주세요. 이것은 강요입니다.",
    };
    
    private AsyncOperation _op;
    private Coroutine _fadeCoroutine;
    private Coroutine _loadingCoroutine;
    private Tween _fade;
    
    private const string LOADING_SCENE = "LoadingScene";
    private const string LOADING_BAR = "LoadingBar";
    //private const string LOADING_ANIMATOR = "LoadingAnimation";
    private const string FADE_IMAGE = "FadeImage";
    private const string LOADING_PERCENT = "LoadingPercent";
    private const string TIP_TEXT = "Tip";

    private void Reset()
    {
        _loadingBar = Helper_Component.FindChildComponent<Image>(this.transform, LOADING_BAR);
        _fadeImage = Helper_Component.FindChildComponent<Image>(this.transform, FADE_IMAGE);
        //_loadingAnimator = Helper_Component.FindChildComponent<Animator>(this.transform, LOADING_ANIMATOR);
        _loadingPercent = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, LOADING_PERCENT);
        _tipText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TIP_TEXT);
    }

    private void Start()
    {
        _fadeImage.color = new Color(0, 0, 0, 0);
        _fadeImage.gameObject.SetActive(false);
        _fadeCoroutine = null;
        _loadingCoroutine = null;
        _loadingCoroutine = StartCoroutine(C_LoadSceneProcess());
        ShowRandomTip();
        PlayRandomAnimation();
    }

    public static void LoadScene(string sceneName)
    {
        NEXT_SCENE = sceneName;
        SceneManager.LoadScene(LOADING_SCENE);
    }

    
    private IEnumerator C_LoadSceneProcess()
    {
        var initTask = GameManager.Instance.InitAsync();
        
        while (GameManager.Instance.InitProgress < 1f)
        {
            _loadingBar.fillAmount = Mathf.Lerp(0f, 0.9f, GameManager.Instance.InitProgress);
            ShowPercentage();
            yield return null;
        }
        
        yield return initTask.AsCoroutine();
        
        _op = SceneManager.LoadSceneAsync(NEXT_SCENE);
        
        if (_op == null)
            Debug.Log("op없음");
        
        _op.allowSceneActivation = false;

        float timer = 0f;
        float minLoadingTime = 1f;
        
        while (!_op.isDone)
        {
            yield return null;

            ShowPercentage();
            
            if (_op.progress < 0.9f)
            {
                _loadingBar.fillAmount = _op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                _loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer / minLoadingTime);

                if (_loadingBar.fillAmount >= 1f)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    _loadingPercent.text = "100.00%";
                    _fadeCoroutine = StartCoroutine(C_FadeOutBeforeScene(_op));
                    yield break;
                }
            }
        }
    }

    
    private IEnumerator C_FadeOutBeforeScene(AsyncOperation op)
    {
        float fadeDuration = 1f;

        _fadeImage.gameObject.SetActive(true);
        
        _fade = _fadeImage.DOFade(1f, fadeDuration);

        yield return _fade.WaitForCompletion();
        _fade.Kill();
        _fade = null;

        op.allowSceneActivation = true;
    }
    
    private void ShowPercentage()
    {
        string percent = (_loadingBar.fillAmount * 100f).ToString("N2");
        _loadingPercent.text = $"{percent}%";
    }
    
    private void ShowRandomTip() 
    {
        string randomTip = _tips[Random.Range(0, _tips.Length)];
        _tipText.text = randomTip;
    }

    private void PlayRandomAnimation()
    {
        int randomAnimation = Random.Range(0, _anims.Length);
        //Debug.Log(randomAnimation);
        // _loadingAnimator.Play(_anims[randomAnimation]);
    }
}