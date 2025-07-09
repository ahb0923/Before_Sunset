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
            DefenseManager.Instance.MonsterSpawner.SpawnMonster(600, 3, false);
        }

        if (GUILayout.Button("몬스터 스폰 : 드리즐"))
        {
            DefenseManager.Instance.MonsterSpawner.SpawnMonster(601, 3, false);
        }

        if (GUILayout.Button("몬스터 스폰 : 헤비"))
        {
            DefenseManager.Instance.MonsterSpawner.SpawnMonster(602, 3, false);
        }
    }
}
#endif