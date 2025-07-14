using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TooltipManager : MonoSingleton<TooltipManager>
{
    [SerializeField] private GameObject _tooltipObject;
    [SerializeField] private TextMeshProUGUI _objectName;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    
    public bool isOpen = false;
    
    private const string TOOLTIP_CONTAINER = "TooltipContainer";
    private const string OBJECT_NAME = "ObjectName";
    private const string TOOLTIP_TEXT = "TooltipText";
    
    protected override void Awake()
    {
        base.Awake();
        
        _tooltipObject = GameObject.Find(TOOLTIP_CONTAINER);
        _objectName = Helper_Component.FindChildByName(_tooltipObject.transform, OBJECT_NAME).GetComponent<TextMeshProUGUI>();
        _tooltipText = Helper_Component.FindChildByName(_tooltipObject.transform, TOOLTIP_TEXT).GetComponent<TextMeshProUGUI>();
        HideTooltip();
    }

    private void Update()
    {
        if (_tooltipObject.activeSelf)
        {
            _tooltipObject.transform.position = Input.mousePosition;
        }
    }

    public void ShowTooltip(string objectName, string description)
    {
        if (!_tooltipObject.activeSelf)
        {
            _tooltipObject.SetActive(true);
            isOpen = true;
            _objectName.text = objectName;
            _tooltipText.text = description;
            
            //친구가 알려준 코드..
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
            isOpen = false;
            _tooltipObject.SetActive(false);
            
            _tooltipObject.transform.position = new Vector3(20000, 20000, 0);
        }
    }
}
