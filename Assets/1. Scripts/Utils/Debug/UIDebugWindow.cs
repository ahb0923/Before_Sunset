#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class UIDebugWindow : EditorWindow
{
    [MenuItem("Tools/UI Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<UIDebugWindow>("UI Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("UI Debug Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("100번 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(100,1);
        }
        
        if (GUILayout.Button("200번 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(200,3);
        }
        
        if (GUILayout.Button("700번 장비 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(700,0);
        }
    }
}
#endif