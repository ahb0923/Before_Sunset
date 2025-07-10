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
        
        if (GUILayout.Button("200번 아이템 추가"))
        {
            InventoryManager.Instance.Inventory.AddItem(200,3);
        }
        
        if (GUILayout.Button("빌드 슬롯 만들기"))
        {
            UIManager.Instance.CraftArea.Work();
        }
        
        if (GUILayout.Button("코루틴 테스트"))
        {
            UIManager.Instance.BattleUI.ShowReturnUI();
        }
        
        if (GUILayout.Button("하급제련소강제주입"))
        {
            UIManager.Instance.SmelterUI.SetSmelterUI(DataManager.Instance.SmelterData.GetById(900));
        }
    }
}
#endif