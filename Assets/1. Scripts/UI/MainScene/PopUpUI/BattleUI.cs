using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("복귀UI")]
    [SerializeField] private GameObject _returnGameObject;
    [SerializeField] private TextMeshProUGUI _returnText;
    [SerializeField] private Image _returnProgressBar;
    
    [Header("전투시작경고UI")]
    [SerializeField] private GameObject _warningGameObject;
    [SerializeField] private Image _warningImage;
    [SerializeField] private TextMeshProUGUI _warningText;
    
    private RectTransform _returnRect;
    private RectTransform _warningRect;
    private Coroutine _dotRoutine;
    private Coroutine _returnRoutine;
    private Tween _imageTween;
    private Tween _textTween;
    private bool isRoutineDone;
    
    private readonly string _baseText = "불길한 기운이 느껴집니다";
    private readonly float _loopTime = 0.4f;
    private readonly float _returnTime = 5f;
    
    private const string RETURN = "Return";
    private const string RETURN_TEXT = "ReturnText";
    private const string RETURN_PROGRESS_BAR = "ReturnProgressBar";
    private const string WARNING = "Warning";
    private const string WARNING_IMAGE = "WarningBG";
    private const string WARNING_TEXT = "WarningText";

    private void Reset()
    {
        _returnGameObject = Helper_Component.FindChildGameObjectByName(this.gameObject, RETURN);
        _returnText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, RETURN_TEXT);
        _returnProgressBar = Helper_Component.FindChildComponent<Image>(this.transform, RETURN_PROGRESS_BAR);
        _warningGameObject = Helper_Component.FindChildGameObjectByName(this.gameObject, WARNING);
        _warningImage = Helper_Component.FindChildComponent<Image>(this.transform, WARNING_IMAGE);
        _warningText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, WARNING_TEXT);
    }

    private void Awake()
    {
        _returnRect = GetComponent<RectTransform>();
        _warningRect = _warningGameObject.GetComponent<RectTransform>();
        _returnGameObject.SetActive(false);
        CloseWarning();
    }

    public void OpenReturn()
    {
        _returnRect.OpenAtCenter();
        _returnGameObject.SetActive(true);
    }

    public void CloseReturn()
    {
        _returnGameObject.SetActive(false);
        _returnRect.CloseAndRestore();
    }

    public void OpenWarning()
    {
        _returnRect.OpenAtCenter();
        _warningRect.OpenAtCenter();
    }

    public void CloseWarning()
    {
        _warningRect.CloseAndRestore();
        _returnRect.CloseAndRestore();
    }
    
    public void ShowReturnUI()
    {
        OpenReturn();
        _returnRoutine = StartCoroutine(C_Return());
    }

    public IEnumerator C_Return()
    {
        StartCoroutine(C_WaitSecond());
        _returnText.text = _baseText;
        StartCoroutine(C_DOTextMesh(_returnText, 1f));
        _returnProgressBar.fillAmount = 0f;
        _returnProgressBar.DOFillAmount(1f, _returnTime).SetEase(Ease.Linear).OnComplete(StopDotRoutine);
        yield return new WaitUntil(() => isRoutineDone);
        isRoutineDone = false;
        StartDotRoutine();
        yield return new WaitUntil(() => _returnRoutine == null);
        CloseReturn();
        if (!DefenseManager.Instance.MainBasePlayer.IsInBase)
        {
            DefenseManager.Instance.MainBasePlayer.InputHandler.StartRecall();

        }
    }
    
    private IEnumerator C_DotRoutine()
    {
        int dotNum = 0;
        
        while (true)
        {
            string dot = new string('.', dotNum);
            _returnText.text = _baseText + dot;
            dotNum++;
            if (dotNum >= 4)
            {
                dotNum = 0;
            }
            yield return new WaitForSeconds(_loopTime);
        }
    }

    private void StartDotRoutine()
    {
        _dotRoutine = StartCoroutine(C_DotRoutine());
    }

    private void StopDotRoutine()
    {
        if (_dotRoutine != null)
        {
            StopCoroutine(_dotRoutine);
            _dotRoutine = null;
            _returnRoutine = null;
        }
    }

    private IEnumerator C_DOTextMesh(TextMeshProUGUI textMesh, float duration)
    {
        textMesh.maxVisibleCharacters = 0;

        Tween tween = DOTween.To(x => textMesh.maxVisibleCharacters = (int)x,
            0f, textMesh.text.Length, duration).SetEase(Ease.Linear);
        yield return tween.WaitForCompletion();
        
        textMesh.maxVisibleCharacters = int.MaxValue;
    }
    
    private IEnumerator C_WaitSecond()
    {
        yield return new WaitForSeconds(1f);
        isRoutineDone = true;
    }

    public void StartWarning()
    {
        if (_textTween != null && _textTween.IsPlaying())
        {
            return;
        }
        
        OpenWarning();
        _imageTween = _warningImage.DOFade(0f, 1f).SetEase(Ease.InOutSine).SetLoops(11, LoopType.Yoyo);
        _textTween = _warningText.DOFade(0f, 1f).SetEase(Ease.InOutSine).SetLoops(11, LoopType.Yoyo).OnComplete(StopWarning);
    }

    public void StopWarning()
    {
        if (_textTween != null || _imageTween != null)
        {
            _imageTween.Kill();
            _imageTween = null;
            
            _textTween.Kill();
            _textTween = null;
            
            Color colorImage = _warningImage.color;
            colorImage.a = 0.9f;
            _warningImage.color = colorImage;
            
            Color color = _warningText.color;
            color.a = 1f;
            _warningText.color = color;
            
            CloseWarning();
        }
    }
}
