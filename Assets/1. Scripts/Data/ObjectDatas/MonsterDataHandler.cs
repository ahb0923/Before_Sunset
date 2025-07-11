using System.Collections.Generic;
using UnityEngine;

public class MonsterDataHandler : BaseDataHandler<MonsterDatabase>
{
    protected override string FileName => "MonsterData_JSON.json";
    protected override int GetId(MonsterDatabase data) => data.id;
    protected override string GetName(MonsterDatabase data) => data.monsterName;

    private Dictionary<int, GameObject> _monsterPrefabs = new();
    public Dictionary<int, GameObject> TowerImages => _monsterPrefabs;
    public GameObject GetPrefabById(int id)
    {
        if (_monsterPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[MonsterDataHandler] ID {id}에 해당하는 프리팹이 존재하지 않습니다.");
        return null;
    }

    public void SettingPrefab()
    {
        foreach (var monster in dataIdDictionary.Values)
        {
            GameObject monsterPrefab = Resources.Load<GameObject>($"Monsters/{monster.prefabName}");
            if (monsterPrefab != null)
            {
                _monsterPrefabs.Add(monster.id, monsterPrefab);
            }
            else
            {
                Debug.LogWarning($"[Setting Prefab] 프리팹 로드 실패: {monster.monsterName} / "); //{monster.prefabName}
            }
        }
        Debug.Log($"[Setting Prefab] 전체 몬스터 프리팹 데이터 ({_monsterPrefabs.Count}개):");
    }

}
