using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour, ICloseableUI
{
    [SerializeField] private RectTransform _saveLoadRect;
    [SerializeField] private Button _exitSaveLoadButton;
    [SerializeField] private Button _cancelButton;
    
    private const string EXIT_SAVE_LOAD_BUTTON = "ExitSaveLoadButton";
    private const string CANCEL_BUTTON = "BackGroundCancelButton";

    private void Reset()
    {
        _saveLoadRect = GetComponent<RectTransform>();
        _exitSaveLoadButton = Helper_Component.FindChildComponent<Button>(this.transform, EXIT_SAVE_LOAD_BUTTON);
        _cancelButton = Helper_Component.FindChildComponent<Button>(this.transform, CANCEL_BUTTON);
        
    }

    private void Awake()
    {
        _exitSaveLoadButton.onClick.AddListener(Close);
        _cancelButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        CloseUI();
    }

    public void Open()
    {
        UIManager.Instance.OpenUI(this);
    }

    public void Close()
    {
        UIManager.Instance.CloseUI(this);
    }

    public void OpenUI()
    {
        _saveLoadRect.OpenAtCenter();
    }

    public void CloseUI()
    {
        _saveLoadRect.CloseAndRestore();
    }
}
