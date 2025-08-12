using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDataHandler : BaseDataHandler<ProjectileDatabase>
{
    //protected override string DataUrl => "https://script.google.com/macros/s/your-tower-sheet-id/exec";

    protected override string FileName => "ProjectileData_JSON.json";
    protected override int GetId(ProjectileDatabase data) => data.id;
    protected override string GetName(ProjectileDatabase data) => data.projectileName;

    private Dictionary<int, GameObject> _projectilePrefabs = new();
    public Dictionary<int, GameObject> TowerImages => _projectilePrefabs;
    public GameObject GetPrefabById(int id)
    {
        if (_projectilePrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        //Debug.LogWarning($"[ProjectileDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }

    protected override void AfterLoaded()
    {
        SettingPrefab();
    }

    /// <summary>
    /// 타워 이미지 데이터 초기화
    /// </summary>
    public void SettingPrefab()
    {
        foreach (var projectile in dataIdDictionary.Values)
        {
            GameObject projectilePrefab = Resources.Load<GameObject>($"Projectiles/{projectile.prefabName}");
            if (projectilePrefab != null)
            {
                _projectilePrefabs.Add(projectile.id, projectilePrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Prefab] 프리팹 로드 실패: {projectile.projectileName} / {projectile.prefabName}");
            }
        }
        Debug.Log($"[Setting  Prefab] 전체 투사체 프리팹 데이터 ({_projectilePrefabs.Count}개):");
    }

    [ContextMenu("Debug all Log")]
    public override void DebugLogAll(Func<ProjectileDatabase, string> formatter = null)
    {
        if (dataIdDictionary.Count == 0)
        {
            Debug.LogWarning("[ProjectileDataHandler] 로드된 타워 데이터가 없습니다.");
            return;
        }

        Debug.Log($"[ProjectileDataHandler] 전체 타워 데이터 ({dataIdDictionary.Count}개):");

        foreach (var projectile in dataIdDictionary.Values)
        {
            if (formatter != null)
            {
                Debug.Log(formatter(projectile));
                continue;
            }
            Debug.Log($"[발사체] ID: {projectile.id}, 이름: {projectile.projectileName}, 프리팹이름: {projectile.prefabName}");
            Debug.Log($"ㄴ발사 속도: {projectile.moveSpeed}");
        }
    }
}
