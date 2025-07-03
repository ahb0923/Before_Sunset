using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    private const string EXIT_SAVE_LOAD_BUTTON = "ExitSaveLoadButton";

    private void Awake()
    {
        Button exitSaveLoadButton = GetComponentInChildren<Button>();
        
        exitSaveLoadButton.onClick.AddListener(Close);
        Close();
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
}
