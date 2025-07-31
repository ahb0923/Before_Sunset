using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AskPopUpUI : MonoBehaviour, ICloseableUI
{
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    [SerializeField] private TextMeshProUGUI _askText;
    
    private RectTransform _askRect;
    
    private Action _onYesAction;
    private Action _onNoAction;
    
    private const string YES_BUTTON = "YesButton";
    private const string NO_BUTTON = "NoButton";
    private const string ASK_TEXT = "AskText";

    private void Reset()
    {
        _yesButton = Helper_Component.FindChildComponent<Button>(this.transform, YES_BUTTON);
        _noButton = Helper_Component.FindChildComponent<Button>(this.transform, NO_BUTTON);
        _askText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, ASK_TEXT);
    }

    /*
     *"기존 저장 데이터를 덮어쓰게 됩니다.\n계속하시겠습니까?"
     */
    
    private void Awake()
    {
        _yesButton.onClick.AddListener(OnClickYes);
        _noButton.onClick.AddListener(OnClickNo);
        
        _askRect = GetComponent<RectTransform>();
    }

    private void OnClickYes()
    {
        _onYesAction?.Invoke();
        Close();
    }
    
    private void OnClickNo()
    {
        _onNoAction?.Invoke();
        Close();
    }

    public void Open(string askText, Action onYesAction = null, Action onNoAction = null)
    {
        _askText.text = askText;
        _onYesAction = onYesAction;
        _onNoAction = onNoAction;

        UIManager.Instance.OpenUI(this);
    }

    public void Open()
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        UIManager.Instance.CloseUI(this);
    }

    public void OpenUI()
    {
        _askRect.OpenAtCenter();
    }

    public void CloseUI()
    {
        _askRect.CloseAndRestore();
    }
}
