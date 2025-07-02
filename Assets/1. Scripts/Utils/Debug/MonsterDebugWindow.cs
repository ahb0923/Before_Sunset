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
            MapManager.Instance.MonsterSpawner.SpawnAllMonsters(0);
        }

        if (GUILayout.Button("몬스터 스폰 : 드리즐"))
        {
            MapManager.Instance.MonsterSpawner.SpawnAllMonsters(1);
        }

        if (GUILayout.Button("몬스터 스폰 : 헤비"))
        {
            MapManager.Instance.MonsterSpawner.SpawnAllMonsters(2);
        }

        if (GUILayout.Button("몬스터 스폰 : 테스트 - 왼쪽에서 소환"))
        {
            MapManager.Instance.MonsterSpawner.SpawnAllMonsters(3, 3);
        }

        if (GUILayout.Button("몬스터 스폰 : 테스트 - 위쪽에서 소환"))
        {
            MapManager.Instance.MonsterSpawner.SpawnAllMonsters(3, 0);
        }
    }
}
#endif