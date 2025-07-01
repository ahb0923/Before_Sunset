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

        if (GUILayout.Button("UI 리셋"))
        {
            Debug.Log("특정 UI를 리셋합니다.");
        }
    }
}
#endif