using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoSingleton<TooltipManager>
{
    [SerializeField] private GameObject _tooltipObject;
    [SerializeField] private GameObject _objectNameGo;
    [SerializeField] private GameObject _tooltipTextGo;
    [SerializeField] private TextMeshProUGUI _objectName;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    
    [SerializeField] private RectTransform _tooltipRect;
    [SerializeField] private Vector2 _defaultOffset = new Vector2(30f, 20f);
    [SerializeField] private Canvas _canvas;
    
    public bool isOpen = false;
    private Vector2 _originalPosition = new Vector2(2000,2000);
    
    private const string TOOLTIP_CONTAINER = "TooltipContainer";
    private const string OBJECT_NAME = "ObjectName";
    private const string TOOLTIP_TEXT = "TooltipText";

    private void Reset()
    {
        _tooltipObject = Helper_Component.FindChildGameObjectByName(this.gameObject, TOOLTIP_CONTAINER);
        _objectNameGo = Helper_Component.FindChildGameObjectByName(this.gameObject, OBJECT_NAME);
        _tooltipTextGo = Helper_Component.FindChildGameObjectByName(this.gameObject, TOOLTIP_TEXT);
        _tooltipRect = Helper_Component.FindChildComponent<RectTransform>(this.transform, TOOLTIP_CONTAINER);
        _objectName = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, OBJECT_NAME);
        _tooltipText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, TOOLTIP_TEXT);
        _canvas = GetComponent<Canvas>();
    }

    protected override void Awake()
    {
        base.Awake();
        
        HideTooltip();
    }

    private void Update()
    {
        if (_tooltipObject.activeSelf)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 anchoredPosition = mousePosition + _defaultOffset;
            
            Vector2 tooltipSize = _tooltipRect.sizeDelta * _canvas.scaleFactor;
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 우측 넘어갈시 좌측으로
            if (mousePosition.x + tooltipSize.x + _defaultOffset.x > screenWidth)
                anchoredPosition.x = mousePosition.x - tooltipSize.x - _defaultOffset.x;

            // 상단 넘어갈시 하단으로
            if (mousePosition.y + tooltipSize.y + _defaultOffset.y > screenHeight)
                anchoredPosition.y = mousePosition.y - tooltipSize.y - _defaultOffset.y;

            _tooltipRect.position = anchoredPosition;
        }
    }

    public void ShowTooltip(string objectName = null, string description = null)
    {
        if (!_tooltipObject.activeSelf)
        {
            isOpen = true;
            _tooltipObject.SetActive(true);
            _objectNameGo.SetActive(true);
            _objectName.text = objectName;

            if (description != null)
            {
                _tooltipTextGo.SetActive(true);
                _tooltipText.text = description;
            }
            
            //레이아웃을 강제적으로 리빌드하는 메서드
            LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipObject.GetComponent<RectTransform>());
        }
    }

    public void UpdateTooltip(string objectName, string description)
    {
        _objectName.text = objectName;
        _tooltipText.text = description;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipObject.GetComponent<RectTransform>());
    }

    public void HideTooltip()
    {
        if (_tooltipObject.activeSelf)
        {
            _objectName.text = "";
            _tooltipText.text = "";
            
            _objectNameGo.SetActive(false);
            _tooltipTextGo.SetActive(false);
            _tooltipObject.SetActive(false);
            
            isOpen = false;
            _tooltipRect.position = _originalPosition;
        }
    }
}
