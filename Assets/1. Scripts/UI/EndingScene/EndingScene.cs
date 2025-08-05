using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    [Header("NarrationText")]
    [SerializeField] private GameObject _endingBackGround;
    [SerializeField] private GameObject _textPrefab;
    [SerializeField] private GameObject _narrationTextContainer;
    [SerializeField] private List<string> _texts = new List<string>
    {
        "어둠은 쉴 틈 없이 몰려왔고",
        "코어의 빛은 점차 약해지고 있었다.",
        "그러나 당신은 물러서지 않았다.",
        "매일을 견디며 코어를 지켜냈다.",
        "드디어, 마지막 밤이 지나갔다.",
        "어둠은 물러났고 코어는 완전히 깨어났다.",
        "당신이 지켜낸 희망의 불씨로 인해",
        "세상은 다시 빛을 되찾았다."
    };
    [SerializeField] private float _textDuration = 2f;
    [SerializeField] private float _moveDuration = 1.5f;
    [SerializeField] private float _moveOffset = 100f;
    
    [Header("BlinkImage")]
    [SerializeField] private GameObject _blinkGameObject;
    [SerializeField] private RectTransform _blinkRect;
    [SerializeField] private Image _blinkImage;
    [SerializeField] private float _blinkDuration = 1f;
    [SerializeField] private Vector3 _defaultOffset = new Vector3(10f, 5f, 0f);
    [SerializeField] private Canvas _canvas;
    
    [Header("FadeImage")]
    [SerializeField] private GameObject _fadeGameObject;
    [SerializeField] private Image _fadeImage;
    
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Volume _globalVolume;
    private Bloom _bloom;

    [SerializeField] private RectTransform _endingCredit;
    [SerializeField] private TextMeshProUGUI _endingCreditText;
    
    private Queue<NarrationText> _activeTexts = new Queue<NarrationText>();
    private Queue<GameObject> _textPool = new Queue<GameObject>();

    private Coroutine _seqRoutine;
    private Coroutine _textRoutine;
    private Tween _blinkTween;
    private Tween _moveTween;

    private const int MAX_TEXT = 4;
    private const string ENDING_BACKGROUND = "EndingBackGround";
    private const string TEXT_PREFAB = "UI/NarrationText";
    private const string OPENING_TEXT_CONTAINER = "NarrationTextContainer";
    private const string BLINK_IMAGE = "BlinkImage";
    private const string FADE_IMAGE = "FadeImage";
    
    private void Reset()
    {
        _endingBackGround = Helper_Component.FindChildGameObjectByName(this.gameObject, ENDING_BACKGROUND);
        _textPrefab = Resources.Load<GameObject>(TEXT_PREFAB);
        _narrationTextContainer = Helper_Component.FindChildGameObjectByName(this.gameObject, OPENING_TEXT_CONTAINER);
        _blinkGameObject = Helper_Component.FindChildGameObjectByName(this.gameObject, BLINK_IMAGE);
        _blinkRect = _blinkGameObject.GetComponent<RectTransform>();
        _blinkImage = _blinkGameObject.GetComponent<Image>();
        _canvas = GetComponent<Canvas>();
        _fadeGameObject = Helper_Component.FindChildGameObjectByName(this.gameObject, FADE_IMAGE);
        _fadeImage = _fadeGameObject.GetComponent<Image>();
    }

    private void Start()
    {
        _fadeImage.color = new Color(0f, 0f, 0f, 0f);
        _fadeGameObject.SetActive(false);
        _blinkGameObject.SetActive(false);
        _endingCreditText.gameObject.SetActive(false);
        _endingCredit.gameObject.SetActive(false);
        InitTexts();
        StartCoroutine(C_StartEnding());
    }

    private void InitTexts()
    {
        for (int i = 0; i < MAX_TEXT + 1; i++)
        {
            var text = Instantiate(_textPrefab, _narrationTextContainer.transform);
            _textPool.Enqueue(text);
            text.SetActive(false);
        }
    }

    private IEnumerator C_StartEnding()
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            _seqRoutine = StartCoroutine(C_Sequence(i));
            yield return _seqRoutine;
        }

        _blinkTween.Kill();
        _blinkTween = null;
        _fadeGameObject.SetActive(true);
        _fadeImage.DOFade(1f, 2f).OnComplete(FadeOut);
    }
    
    private IEnumerator C_Sequence(int i)
    {
        var go = GetText();
        var text = go.GetComponent<NarrationText>();
        _activeTexts.Enqueue(text);
        text.ResetNarrationText();
        
        text.textMesh.text = _texts[i];
        _textRoutine = StartCoroutine(text.C_DOTextMesh(_textDuration));
        yield return _textRoutine;
        
        BlinkTransform(text.rect);
        Blink();
        while (_blinkTween.IsPlaying())
        {
            if (Input.GetMouseButtonDown(0) ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return))
            {
                _blinkTween.Kill();
                _blinkTween = null;
                _blinkGameObject.SetActive(false);
                break;
            }
            yield return null;
        }

        if (i == _texts.Count - 1)
        {
            yield break;
        }

        if (_activeTexts.Count >= MAX_TEXT)
        {
            var lastText = _activeTexts.Dequeue();
            ReturnText(lastText.gameObject);
        }

        foreach (var ot in _activeTexts)
        {
            ot.MoveText(_moveOffset, _moveDuration);
        }
        
        yield return new WaitForSeconds(0.2f);
    }

    private void Blink()
    {
        Color color = _blinkImage.color;
        color.a = 1f;
        _blinkImage.color = color;
        
        _blinkGameObject.SetActive(true);
        _blinkTween = _blinkImage.DOFade(0f, _blinkDuration).SetLoops(-1, LoopType.Yoyo);
    }

    private void BlinkTransform(RectTransform rt)
    {
        Vector2 ps = _defaultOffset;
        Vector2 textSize = rt.sizeDelta * _canvas.scaleFactor;
        
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        ps.x += (textSize.x / 2) + (screenWidth / 2);
        ps.y += (screenHeight / 2) - (textSize.y / 2);
        _blinkRect.position = ps;
    }

    private void FadeOut()
    {
        _endingBackGround.SetActive(false);

        _fadeImage.DOFade(0f, 3f).OnComplete(ZoomAndGlow);
    }

    private void ZoomAndGlow()
    {
        _globalVolume.profile.TryGet<Bloom>(out _bloom);
        
        float start = 0f;
        float end = 17f;
        
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(PlayHeartBeat);
        seq.AppendInterval(0.2f);
        seq.Append(_mainCamera.DOFieldOfView(50f, 15f).SetEase(Ease.Linear));
        seq.Join(DOVirtual.Float(start, end, 0.5f, val => _bloom.intensity.value = val).SetLoops(42, LoopType.Yoyo).OnComplete(StartEndingCredit));
    }

    private void StartEndingCredit()
    {
        StopHeartBeat();
        _endingCredit.gameObject.SetActive(true);
        _endingCreditText.maxVisibleCharacters = 0;
        _endingCreditText.gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(PlayBGM);
        seq.AppendInterval(5f);
        seq.Append(DOTween.To(x => _endingCreditText.maxVisibleCharacters = (int)x,
            0f, _endingCreditText.text.Length, 1f).SetEase(Ease.Linear));
        seq.AppendInterval(1f);
        seq.Append(_endingCredit.DOAnchorPosY(8000f, 40f).SetEase(Ease.Linear).OnComplete(LoadScene));
    }

    private void PlayHeartBeat()
    {
        AudioManager.Instance.SetSFXVolume(1f);
        AudioManager.Instance.PlaySFX("HeartBeat");
    }

    private void StopHeartBeat()
    {
        AudioManager.Instance.SetSFXMute(true);
    }

    private void PlayBGM()
    {
        AudioManager.Instance.SetBGMVolume(1f);
        AudioManager.Instance.PlayBGM("NormalBase");
    }
    
    private void LoadScene()
    {
        SceneManager.LoadScene("StartScene");
    }
    
    private GameObject GetText()
    {
        if (_textPool.Count > 0)
        {
            var t = _textPool.Dequeue();
            t.gameObject.SetActive(true);
            return t;
        }

        return Instantiate(_textPrefab, _narrationTextContainer.transform);
    }

    private void ReturnText(GameObject text)
    {
        text.SetActive(false);
        _textPool.Enqueue(text);
    }
}
