using System;
using UnityEngine;
using UnityEngine.UI;

public class RefineryUI : MonoBehaviour
{
    [SerializeField] private GameObject _refineryInfoArea;
    
    public RefineryInfoUI RefineryInfoUI { get; private set; }
    
    private const string REFINERY_INFO_AREA = "RefineryInfoArea";
    private const string REFINERY_BUTTON = "RefineryButton";

    private void Reset()
    {
        _refineryInfoArea = transform.Find(REFINERY_INFO_AREA).gameObject;
    }

    private void Awake()
    {
        Button refineryButton = UtilityLJH.FindChildComponent<Button>(this.transform.parent, REFINERY_BUTTON);
        refineryButton.onClick.AddListener(ToggleRefinery);
        RefineryInfoUI = _refineryInfoArea.GetComponent<RefineryInfoUI>();
        this.gameObject.SetActive(false);
    }

    private void ToggleRefinery()
    {
        bool isOpen = this.gameObject.activeSelf;
        this.gameObject.SetActive(!isOpen);

        if (!isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    
    private void Open()
    {
        this.gameObject.SetActive(true);

        if (_refineryInfoArea != null)
        {
            _refineryInfoArea.SetActive(false);
        }
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
}
