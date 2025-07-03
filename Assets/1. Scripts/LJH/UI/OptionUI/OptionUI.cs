using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    private const string OPTION_BUTTON = "OptionButton";
    private const string UN_PAUSE_BUTTON = "UnPauseButton";
    
    private void Awake()
    {
        Button optionButton = Helper_Component.FindChildComponent<Button>(this.transform.parent, OPTION_BUTTON);
        Button unPauseButton = Helper_Component.FindChildComponent<Button>(this.transform, UN_PAUSE_BUTTON);
        optionButton.onClick.AddListener(Open);
        unPauseButton.onClick.AddListener(Close);
        
        Close();
    }

    private void Open()
    {
        this.gameObject.SetActive(true);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
}
