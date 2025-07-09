using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    public RectTransform saveLoadRect;
    
    private const string EXIT_SAVE_LOAD_BUTTON = "ExitSaveLoadButton";

    private void Awake()
    {
        Button exitSaveLoadButton = GetComponentInChildren<Button>();
        exitSaveLoadButton.onClick.AddListener(Close);
        saveLoadRect = GetComponent<RectTransform>();
    }

    private void Close()
    {
        saveLoadRect.CloseAndRestore();
    }
}
