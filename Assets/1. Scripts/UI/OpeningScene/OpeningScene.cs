using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OpeningScene : MonoBehaviour
{
    [Header("OpeningText")]
    [SerializeField] private GameObject _textPrefab;
    [SerializeField] private GameObject _openingTextContainer;
    [SerializeField] private List<string> _texts = new List<string>
    {
        "어느 날, 어둠 속에서 태어난 검은 원념의 존재들이 지상을 뒤덮기 시작했다.",
        "‘나’는 폐허가 된 마을을 헤매다 지하 깊숙한 곳에서 새어나오는 빛의 틈새에 이끌렸다.",
        "그 곳엔 오래전 잊혀진 빛나는 구, “코어”가 있었다.",
        "그 빛은 원념의 존재들에게 강한 저항력을 가진 유일한 힘이었다.",
    };
    [SerializeField] private float _textDuration = 1f;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private float _moveOffset = 50f;
    
    [Header("BlinkImage")]
    [SerializeField] private GameObject _blinkGo;
    [SerializeField] private RectTransform _blinkRect;
    [SerializeField] private Image _blinkImage;
    [SerializeField] private float _blinkDuration = 0.5f;
    [SerializeField] private Vector3 _defaultOffset = new Vector3(10f, 5f, 0f);
    [SerializeField] private Canvas _canvas;
    
    private Queue<OpeningText> _activeTexts = new Queue<OpeningText>();
    private Queue<GameObject> _textPool = new Queue<GameObject>();

    public bool isPlayed;
    private Coroutine _seqRoutine;
    private Coroutine _textRoutine;
    private Tween _blinkTween;
    private Tween _moveTween;
    
    private const string TEXT_PREFAB = "UI/OpeningText";
    private const string OPENING_TEXT_CONTAINER = "OpeningTextContainer";
    private const string BLINK_IMAGE = "BlinkImage";
    
    private void Reset()
    {
        _textPrefab = Resources.Load<GameObject>(TEXT_PREFAB);
        _openingTextContainer = Helper_Component.FindChildGameObjectByName(this.gameObject, OPENING_TEXT_CONTAINER);
        _blinkGo = Helper_Component.FindChildGameObjectByName(this.gameObject, BLINK_IMAGE);
        _blinkRect = _blinkGo.GetComponent<RectTransform>();
        _blinkImage = _blinkGo.GetComponent<Image>();
        _canvas = GetComponent<Canvas>();
    }

    private void Awake()
    {
        _blinkGo.SetActive(false);
        InitTexts();

        StartCoroutine(C_StartOpening());
    }

    private void InitTexts()
    {
        for (int i = 0; i < _texts.Count; i++)
        {
            var text = Instantiate(_textPrefab, _openingTextContainer.transform);
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
        LoadingSceneController.LoadScene("MainScene");
    }
    
    private IEnumerator C_Sequence(int i)
    {
        var go = GetText();
        var text = go.GetComponent<OpeningText>();
        _activeTexts.Enqueue(text);
        
        text.textMesh.text = _texts[i];
        _textRoutine = StartCoroutine(text.C_DOTextMesh(_textDuration));
        yield return _textRoutine;
        
        BlinkTransform(text.rect);
        Blink();
        while (_blinkTween != null)
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
            isPlayed = true;
            yield break;
        }

        foreach (var ot in _activeTexts)
        {
            ot.MoveText(_moveOffset, _moveDuration);
        }
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

        return Instantiate(_textPrefab, _openingTextContainer.transform);
    }
    
    public void ReturnText(GameObject text)
    {
        text.gameObject.SetActive(false);
        _textPool.Enqueue(text);
    }
}
