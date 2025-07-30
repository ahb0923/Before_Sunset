using UnityEditor;
using UnityEngine;

public class UpgradeDebugWindow : EditorWindow
{
    //[MenuItem("Tools/Upgrade Debug")]
    //public static void ShowWindow()
    //{
    //    GetWindow<UpgradeDebugWindow>("Upgrade Debug");
    //}

    //private void OnGUI()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        EditorGUILayout.HelpBox("게임이 실행 중일 때만 사용 가능합니다.", MessageType.Info);
    //        return;
    //    }

    //    GUILayout.Label("플레이어 업그레이드", EditorStyles.boldLabel);

    //    DrawPlayerUpgradeButton("Move Speed", PLAYER_STATUS_TYPE.MoveSpeed);
    //    DrawPlayerUpgradeButton("Mining Speed", PLAYER_STATUS_TYPE.MiningSpeed);
    //    DrawPlayerUpgradeButton("Drop Rate", PLAYER_STATUS_TYPE.DropRate);
    //    DrawPlayerUpgradeButton("Sight Range", PLAYER_STATUS_TYPE.SightRange);

    //    GUILayout.Space(20);
    //    GUILayout.Label("코어 업그레이드", EditorStyles.boldLabel);

    //    DrawCoreUpgradeButton("HP", CORE_STATUS_TYPE.HP);
    //    DrawCoreUpgradeButton("Attack Range", CORE_STATUS_TYPE.AttackRange);
    //    DrawCoreUpgradeButton("Attack Damage", CORE_STATUS_TYPE.AttackDamage);
    //    DrawCoreUpgradeButton("Sight Range", CORE_STATUS_TYPE.SightRange);
    //}

    //private void DrawPlayerUpgradeButton(string label, PLAYER_STATUS_TYPE type)
    //{
    //    if (GUILayout.Button($"플레이어 {label} 업그레이드"))
    //    {
    //        bool result = UpgradeManager.Instance.TryUpgradePlayer(type);
    //        if (!result)
    //            Debug.LogWarning($"{label} 업그레이드 실패");
    //    }

    //    if (UpgradeManager.Instance != null)
    //    {
    //        int level = UpgradeManager.Instance.PlayerStatus[type];
    //        float effect = UpgradeManager.Instance.GetCurrentPlayerUpgradeEffect(type);
    //        GUILayout.Label($"레벨: {level} / 효과: +{effect}");
    //    }
    //}

    //private void DrawCoreUpgradeButton(string label, CORE_STATUS_TYPE type)
    //{
    //    if (GUILayout.Button($"코어 {label} 업그레이드"))
    //    {
    //        bool result = UpgradeManager.Instance.TryUpgradeCore(type);
    //        if (!result)
    //            Debug.LogWarning($"{label} 업그레이드 실패");
    //    }

    //    if (UpgradeManager.Instance != null)
    //    {
    //        int level = UpgradeManager.Instance.CoreStatus[type];
    //        float effect = UpgradeManager.Instance.GetCurrentCoreUpgradeEffect(type);
    //        GUILayout.Label($"레벨: {level} / 효과: +{effect}");
    //    }
    //}
}
