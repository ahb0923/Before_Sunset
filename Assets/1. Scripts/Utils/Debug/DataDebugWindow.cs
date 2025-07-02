#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class DataDebugWindow : EditorWindow
{
    [MenuItem("Tools/Data Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<DataDebugWindow>("Data Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("DataManager Debug Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Log All Minerals"))
        {
            DataManager.Instance.MineralData.DebugLogAll();
        }

        if (GUILayout.Button("Log All Towers"))
        {
            DataManager.Instance.TowerData.DebugLogAll();
        }

        if (GUILayout.Button("Log All Monsters"))
        {
            DataManager.Instance.MonsterData.DebugLogAll();
        }

        if (GUILayout.Button("Log All Jewels"))
        {
            DataManager.Instance.JewelData.DebugLogAll();
        }

        if (GUILayout.Button("Log All Ores"))
        {
            DataManager.Instance.OreData.DebugLogAll();
        }

        if (GUILayout.Button("Log All Equipment"))
        {
            DataManager.Instance.EquipmentData.DebugLogAll();
        }
    }
}
#endif