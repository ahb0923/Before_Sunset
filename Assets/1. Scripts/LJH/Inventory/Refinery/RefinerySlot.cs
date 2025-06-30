using System;
using UnityEngine;
using UnityEngine.UI;

public class RefinerySlot : MonoBehaviour
{
    private int _index;
    
    [SerializeField] private Button _refineryButton;
    [SerializeField] private Image _refineryIcon;
    
    private const string REFINERY_ICON = "RefineryIcon";

    private void Reset()
    {
        _refineryButton = this.gameObject.GetComponent<Button>();
        _refineryIcon = UtilityLJH.FindChildComponent<Image>(this.transform, REFINERY_ICON);
    }

    public void SetSlot(int slotIndex)
    {
        SetIndex(slotIndex);
        SetUI();
        _refineryButton.onClick.AddListener(OpenRefineryInfo);
    }
    
    private void SetIndex(int index)
    {
        _index = index;
    }

    private void SetUI()
    {
        //_refineryIcon = refineryData[index].icon
    }

    private void OpenRefineryInfo()
    {
        //UIManager.Instance.refineryInfoUI.Open();
        //UIManager.Instance.refineryInfoUI.UpdateUI(refineryData[index]);
    }
}
