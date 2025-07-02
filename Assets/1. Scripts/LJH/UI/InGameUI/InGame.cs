using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class InGame : MonoBehaviour
{
    private const string OPTION_BUTTON = "OptionButton";
    private const string INVENTORY_BUTTON = "InventoryButton";
    private const string RETURN_BUTTON = "ReturnButton";
    private const string BUILD_BUTTON = "BuildButton";
    private const string UN_BUILD_BUTTON = "UnBuildButton";

    private void Awake()
    {
        Button optionButton = UtilityLJH.FindChildComponent<Button>(this.transform.parent, OPTION_BUTTON);
        Button inventoryButton = UtilityLJH.FindChildComponent<Button>(this.transform, INVENTORY_BUTTON);
        Button returnButton = UtilityLJH.FindChildComponent<Button>(this.transform, RETURN_BUTTON);
        Button buildButton = UtilityLJH.FindChildComponent<Button>(this.transform, BUILD_BUTTON);
        Button unBuildButton = UtilityLJH.FindChildComponent<Button>(this.transform, UN_BUILD_BUTTON);
        
        //optionButton.onClick.AddListener();
        //inventoryButton.onClick.AddListener();
        //returnButton.onClick.AddListener();
        //buildButton.onClick.AddListener();
        //unBuildButton.onClick.AddListener();
    }
}