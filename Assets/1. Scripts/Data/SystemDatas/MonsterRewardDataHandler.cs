using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRewardDataHandler : BaseDataHandler<MonsterRewardDatabase>
{
    protected override string FileName => "MonsterRewardData_JSON.json";
    protected override int GetId(MonsterRewardDatabase data) => data.id;
    protected override string GetName(MonsterRewardDatabase data) => data.idName;

    private Dictionary<int, List<MonsterRewardDatabase>> _monsterRewardDictionary = new();


    protected override void AfterLoaded()
    {
        BuildMonsterRewardDictionary();
    }

    private void BuildMonsterRewardDictionary()
    {
        foreach (var reward in dataIdDictionary.Values)
        {
            if (!_monsterRewardDictionary.ContainsKey(reward.monsterid))
                _monsterRewardDictionary[reward.monsterid] = new List<MonsterRewardDatabase>();

            _monsterRewardDictionary[reward.monsterid].Add(reward);
        }
    }

    /// <summary>
    /// 몬스터 id 입력하면 몬스터가 드랍하는 모든 아이템들의 정보를 List로 받아옴
    /// </summary>
    /// <param name="monsterId"></param>
    /// <returns></returns>
    public List<MonsterRewardDatabase> GetMonsterRewardsByMonsterId(int monsterId)
    {
        if (_monsterRewardDictionary.TryGetValue(monsterId, out var rewards))
            return rewards;

        return new List<MonsterRewardDatabase>();
    }

}
