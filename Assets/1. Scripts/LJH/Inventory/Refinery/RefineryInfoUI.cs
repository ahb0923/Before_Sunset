using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RefineryInfoUI : MonoBehaviour
{
    [SerializeField] private Image _refineryIcon;
    [SerializeField] private Image _firstMaterialIcon;
    [SerializeField] private Image _secondMaterialIcon;
    [SerializeField] private Image _thirdMaterialIcon;
    [SerializeField] private TextMeshProUGUI _refineryName;
    [SerializeField] private TextMeshProUGUI _firstMaterialAmount;
    [SerializeField] private TextMeshProUGUI _secondMaterialAmount;
    [SerializeField] private TextMeshProUGUI _thirdMaterialAmount;
    
    private const string REFINERY_ICON = "RefineryIcon";
    private const string FIRST_MATERIAL_ICON = "FirstMaterialIcon";
    private const string SECOND_MATERIAL_ICON = "SecondMaterialIcon";
    private const string THIRD_MATERIAL_ICON = "ThirdMaterialIcon";
    private const string REFINERY_NAME = "RefineryName";
    private const string FIRST_MATERIAL_AMOUNT = "FirstMaterialAmount";
    private const string SECOND_MATERIAL_AMOUNT = "SecondMaterialAmount";
    private const string THIRD_MATERIAL_AMOUNT = "ThirdMaterialAmount";
    
    private const string EXIT_BUTTON = "ExitButton";
    private const string CRAFT_BUTTON = "CraftButton";

    private void Reset()
    {
        _refineryIcon = UtilityLJH.FindChildComponent<Image>(this.transform, REFINERY_ICON);
        _firstMaterialIcon = UtilityLJH.FindChildComponent<Image>(this.transform, FIRST_MATERIAL_ICON);
        _secondMaterialIcon = UtilityLJH.FindChildComponent<Image>(this.transform, SECOND_MATERIAL_ICON);
        _thirdMaterialIcon = UtilityLJH.FindChildComponent<Image>(this.transform, THIRD_MATERIAL_ICON);
        _refineryName = UtilityLJH.FindChildComponent<TextMeshProUGUI>(this.transform, REFINERY_NAME);
        _firstMaterialAmount = UtilityLJH.FindChildComponent<TextMeshProUGUI>(this.transform, FIRST_MATERIAL_AMOUNT);
        _secondMaterialAmount = UtilityLJH.FindChildComponent<TextMeshProUGUI>(this.transform, SECOND_MATERIAL_AMOUNT);
        _thirdMaterialAmount = UtilityLJH.FindChildComponent<TextMeshProUGUI>(this.transform, THIRD_MATERIAL_AMOUNT);
    }

    private void Awake()
    {
        Button exitButton = UtilityLJH.FindChildComponent<Button>(this.transform, EXIT_BUTTON);
        Button craftButton = UtilityLJH.FindChildComponent<Button>(this.transform, CRAFT_BUTTON);
        exitButton.onClick.AddListener(Close);
        //craftButton.onClick.AddListener();
    }

    public void UpdateUI()
    {
        
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
