using System;
using TMPro;
using UnityEngine;

public class SmelterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _smelterNameText;
    [SerializeField] public SmelterSlot smelterInputSlot;
    [SerializeField] public SmelterSlot smelterOutputSlot;
    
    private RectTransform _rect;

    private const string SMELTER_NAME_TEXT = "SmelterNameText";
    private const string SMELTER_INPUT_SLOT = "SmelterInputSlot";
    private const string SMELTER_OUTPUT_SLOT = "SmelterOutputSlot";

    private void Reset()
    {
        _smelterNameText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, SMELTER_NAME_TEXT);
        smelterInputSlot = Helper_Component.FindChildComponent<SmelterSlot>(this.transform, SMELTER_INPUT_SLOT);
        smelterOutputSlot = Helper_Component.FindChildComponent<SmelterSlot>(this.transform, SMELTER_OUTPUT_SLOT);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        smelterInputSlot.InitInputSlot(true);
        smelterOutputSlot.InitInputSlot(false);
    }

    public void OpenSmelter()
    {
        _rect.OpenAtCenter();
    }

    public void CloseSmelter()
    {
        _rect.CloseAndRestore();
    }
}
