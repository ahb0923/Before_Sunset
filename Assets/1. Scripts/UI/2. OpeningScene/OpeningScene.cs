using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningScene : MonoBehaviour
{
    [Header("OpeningText")]
    [SerializeField] private GameObject _textPrefab;
    [SerializeField] private GameObject _narrationTextContainer;
    [SerializeField] private List<string> _texts = new List<string>
    {
        "어느 날, 어둠 속에서 태어난 검은 원념의 존재들이 지상을 뒤덮기 시작했다.",
        "‘나’는 폐허가 된 마을을 헤매다 지하 깊숙한 곳에서 새어나오는 빛의 틈새에 이끌렸다.",
        "그 곳엔 오래전 잊혀진 빛나는 구, “코어”가 있었다.",
        "그 빛은 원념의 존재들에게 강한 저항력을 가진 유일한 힘이었다.",
        "그러나 빛의 힘은 약해지는 중이었고 어둠은 지하까지 손을 뻗치기 시작한다.",
        "매일 낮, 빛이 희미하게 남아있는 시간에 자원을 모으고,",
        "코어의 힘이 약해지는 밤이 오면 코어를 파괴하려는 몬스터의 공격을 막아내야 한다.",
        "코어가 멈추는 순간, 마지막 희망마저 사라진다."
    };
    [SerializeField] private float _textDuration = 3f;
    [SerializeField] private float _moveDuration = 1.5f;
    [SerializeField] private float _moveOffset = 100f;
    
    [Header("BlinkImage")]
    [SerializeField] private GameObject _blinkGo;
    [SerializeField] private RectTransform _blinkRect;
    [SerializeField] private Image _blinkImage;
    [SerializeField] private float _blinkDuration = 1f;
    [SerializeField] private Vector3 _defaultOffset = new Vector3(10f, 5f, 0f);
    [SerializeField] private Canvas _canvas;
    
    [Header("FadeIn")]
    [SerializeField] private GameObject _fadeInGo;
    [SerializeField] private Image _fadeInImage;
    
    private Queue<NarrationText> _activeTexts = new Queue<NarrationText>();
    private Queue<GameObject> _textPool = new Queue<GameObject>();

    private Coroutine _seqRoutine;
    private Coroutine _textRoutine;
    private Tween _blinkTween;
    private Tween _moveTween;

    private const int MAX_TEXT = 4;
    private const string TEXT_PREFAB = "UI/NarrationText";
    private const string OPENING_TEXT_CONTAINER = "NarrationTextContainer";
    private const string BLINK_IMAGE = "BlinkImage";
    private const string FADE_IN = "FadeIn";
    
    private void Reset()
    {
        _textPrefab = Resources.Load<GameObject>(TEXT_PREFAB);
        _narrationTextContainer = Helper_Component.FindChildGameObjectByName(this.gameObject, OPENING_TEXT_CONTAINER);
        _blinkGo = Helper_Component.FindChildGameObjectByName(this.gameObject, BLINK_IMAGE);
        _blinkRect = _blinkGo.GetComponent<RectTransform>();
        _blinkImage = _blinkGo.GetComponent<Image>();
        _canvas = GetComponent<Canvas>();
        _fadeInGo = Helper_Component.FindChildGameObjectByName(this.gameObject, FADE_IN);
        _fadeInImage = _fadeInGo.GetComponent<Image>();
    }

    private void Start()
    {
        _fadeInImage.color = new Color(0f, 0f, 0f, 0f);
        _fadeInGo.SetActive(false);
        _blinkGo.SetActive(false);
        InitTexts();
        AudioManager.Instance.StopAllSound();
        AudioManager.Instance.SetSFXVolume(1f);
        AudioManager.Instance.SetBGMVolume(1f);
        StartCoroutine(C_StartOpening());
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

    private IEnumerator C_StartOpening()
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            _seqRoutine = StartCoroutine(C_Sequence(i));
            yield return _seqRoutine;
        }

        _blinkTween.Kill();
        _blinkTween = null;
        _fadeInGo.SetActive(true);
        _fadeInImage.DOFade(1f, 2f).OnComplete(LoadScene);
    }

    private void LoadScene()
    {
        SaveManager.Instance.LoadGameFromSlot();
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
                _blinkGo.SetActive(false);
                break;
            }
            yield return null;
        }

        if (i == _texts.Count - 1)
        {
            GlobalState.HasPlayedOpening = true;
            yield break;
        }

        if (_activeTexts.Count >= MAX_TEXT)
        {
            var openingText = _activeTexts.Dequeue();
            ReturnText(openingText.gameObject);
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
        
        _blinkGo.SetActive(true);
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
