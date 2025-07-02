using TMPro;
using UnityEngine;

public class TooltipManager : MonoSingleton<TooltipManager>
{
    [SerializeField] private GameObject _tooltipObject;
    [SerializeField] private TextMeshProUGUI _objectName;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    
    private const string TOOLTIP_CONTAINER = "TooltipContainer";
    private const string OBJECT_NAME = "ObjectName";
    private const string TOOLTIP_TEXT = "TooltipText";
    
    protected override void Awake()
    {
        base.Awake();
        
        _tooltipObject = GameObject.Find(TOOLTIP_CONTAINER);
        _objectName = Helper_Component.GetComponentInChildren<TextMeshProUGUI>(_tooltipObject, OBJECT_NAME);
        _tooltipText = Helper_Component.GetComponentInChildren<TextMeshProUGUI>(_tooltipObject, TOOLTIP_TEXT);
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
            _objectName.text = objectName;
            _tooltipText.text = description;
        }
    }

    public void HideTooltip()
    {
        if (_tooltipObject.activeSelf)
        {
        _tooltipObject.SetActive(false);
        _objectName.text = "";
        _tooltipText.text = "";
        }
    }
}
