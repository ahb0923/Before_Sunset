#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class PlayerDebugWindow : EditorWindow
{
    [MenuItem("Tools/Player Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<PlayerDebugWindow>("Player Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("Player Debug Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("플레이어 레벨 리셋"))
        {
            Debug.Log("플레이어 레벨을 리셋합니다.");
        }
    }
}
#endif