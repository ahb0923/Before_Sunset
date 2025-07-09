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
        
        if (GUILayout.Button("100번 아이템 30개 사용"))
        {
            bool used = InventoryManager.Instance.Inventory.UseItem(100,30);
            if (used)
            {
                Debug.Log("사용!");
            }
            else
            {
                Debug.Log("사용못함!");
            }
        }
        
        if (GUILayout.Button("200번 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(200,3);
        }
        
        if (GUILayout.Button("700번 장비 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(700,0);
        }
        
        if (GUILayout.Button("빌드 슬롯 만들기"))
        {
            UIManager.Instance.CraftArea.Work();
        }
        
        if (GUILayout.Button("코루틴 테스트"))
        {
            UIManager.Instance.BattleUI.ShowReturnUI();
        }
        
        if (GUILayout.Button("루프 테스트"))
        {
            UIManager.Instance.BattleUI.StartBlink();
        }
    }
}
#endif