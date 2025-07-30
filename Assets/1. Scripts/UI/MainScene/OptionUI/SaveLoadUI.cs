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
        saveLoadRect = GetComponent<RectTransform>();
        exitSaveLoadButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        saveLoadRect.CloseAndRestore();
    }
}
