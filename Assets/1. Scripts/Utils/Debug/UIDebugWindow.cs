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

        if (GUILayout.Button("100번 아이템 추가(돌)"))
        {
            InventoryManager.Instance.Inventory.AddItem(100,7);
        }
        
        if (GUILayout.Button("110번 아이템 추가(구리)"))
        {
            InventoryManager.Instance.Inventory.AddItem(110,9);
        }
        
        if (GUILayout.Button("120번 아이템 추가(철)"))
        {
            InventoryManager.Instance.Inventory.AddItem(120,9);
        }
        
        if (GUILayout.Button("111번 아이템 추가(구리주괴)"))
        {
            InventoryManager.Instance.Inventory.AddItem(111,20);
        }
        
        if (GUILayout.Button("121번 아이템 추가(철주괴)"))
        {
            InventoryManager.Instance.Inventory.AddItem(121,20);
        }
        
        if (GUILayout.Button("131번 아이템 추가(은주괴)"))
        {
            InventoryManager.Instance.Inventory.AddItem(131,20);
        }
        
        if (GUILayout.Button("141번 아이템 추가(금주괴)"))
        {
            InventoryManager.Instance.Inventory.AddItem(141,20);
        }
        
        if (GUILayout.Button("업그레이드창 열기"))
        {
            UIManager.Instance.UpgradeUI.Open();
        }
        
        if (GUILayout.Button("정수 4개 추가"))
        {
            UpgradeManager.Instance.AddEssencePiece(12000);
        }
    }
}
#endif