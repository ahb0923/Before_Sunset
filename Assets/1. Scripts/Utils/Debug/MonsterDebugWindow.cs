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

        if (GUILayout.Button("몬스터 스폰 : 머프"))
        {
            FindObjectOfType<MonsterSpawner>().SpawnAllMonsters(0);
        }

        if (GUILayout.Button("몬스터 스폰 : 드리즐"))
        {
            FindObjectOfType<MonsterSpawner>().SpawnAllMonsters(1);
        }

        if (GUILayout.Button("몬스터 스폰 : 헤비"))
        {
            FindObjectOfType<MonsterSpawner>().SpawnAllMonsters(2);
        }
    }
}
#endif