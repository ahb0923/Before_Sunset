#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class MonsterDebugWindow : EditorWindow
{
    [MenuItem("Tools/Monster Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<MonsterDebugWindow>("Monster Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("Monster Debug Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("몬스터 전체 제거"))
        {
            Debug.Log("몬스터가 전부 제거 됩니다.");
        }
    }
}
#endif