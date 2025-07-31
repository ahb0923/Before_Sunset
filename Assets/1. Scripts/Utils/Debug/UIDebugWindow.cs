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
            InventoryManager.Instance.Inventory.AddItem(100,7);
        }
        
        if (GUILayout.Button("110번 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(110,9);
        }
        
        if (GUILayout.Button("120번 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(120,9);
        }
        
        if (GUILayout.Button("110번 아이템 15개 사용"))
        {
            bool used = InventoryManager.Instance.Inventory.UseItem(110,15);
            if (used)
            {
                Debug.Log("사용!");
            }
            else
            {
                Debug.Log("사용못함!");
            }
        }
        
        if (GUILayout.Button("111번 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(111,20);
        }
        
        if (GUILayout.Button("업그레이드창 열기"))
        {
            UIManager.Instance.UpgradeUI.Open();
        }
        
        if (GUILayout.Button("정수 4개 추가"))
        {
            UpgradeManager.Instance.AddEssencePiece(120);
        }
        
        if (GUILayout.Button("정수조각 4개 추가"))
        {
            UpgradeManager.Instance.AddEssencePiece(4);
        }
    }
}
#endif