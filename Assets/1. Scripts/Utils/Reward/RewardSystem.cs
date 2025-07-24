using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardSystem : PlainSingleton<RewardSystem>
{
    /// <summary>
    /// 몬스터 ID에 따른 모든 보상 생성
    /// </summary>
    public void GenerateRewards(int monsterId, Vector3 monsterPos)
    {
        var rewards = DataManager.Instance.MonsterRewardData.GetMonsterRewardsByMonsterId(monsterId);
        foreach(var reward in rewards)
        {
            GetRewardItemId(reward, monsterPos);
        }
    }

    /// <summary>
    /// 확률에 의한 보상 아이템 ID 반환<br/>
    /// ※ 반환값 -1은 확률을 뚫지 못해서 해당 보상을 얻지 못하는 것을 의미
    /// </summary>
    private void GetRewardItemId(MonsterRewardDatabase reward, Vector3 monsterPos)
    {
        if (!CanDrop(reward.dropProbability))
            return;

        int quantity = GetRandomQuantity(reward.minQuantity, reward.maxQuantity);
        switch (reward.rewardType)
        {
            case MONSTER_REWARD_TYPE.EssenceShard:
                // 여기는 이펙트 필요
                Debug.Log("[Reward] 샤드 보상");
                break;

            case MONSTER_REWARD_TYPE.Mineral:
                List<MineralDatabase> minerals;
                if(TimeManager.Instance.Stage == 1) // 여기 하드 코딩, 데이터 베이스 수정 필요
                {
                    minerals = DataManager.Instance.MineralData.GetAllItems()
                        .Where(item => item.itemType == MINERAL_TYPE.Mineral).Where(item => item.id < 120).ToList();
                }
                else
                {
                    minerals = DataManager.Instance.MineralData.GetAllItems().Where(item => item.itemType == MINERAL_TYPE.Mineral).ToList();
                }
                PoolManager.Instance.GetFromPool(minerals[Random.Range(0, minerals.Count)].id, monsterPos);
                break;

            case MONSTER_REWARD_TYPE.Jewel:
                List<JewelDatabase> jewels = DataManager.Instance.JewelData.GetAllItems();
                GameObject obj = PoolManager.Instance.GetFromPool(jewels[Random.Range(0, jewels.Count)].id, monsterPos);
                obj.GetComponent<JewelController>().Interact();
                break;

            case MONSTER_REWARD_TYPE.Ingot:
                List<MineralDatabase> ingots;
                if (TimeManager.Instance.Stage == 1) // 여기 하드 코딩, 데이터 베이스 수정 필요
                {
                    ingots = DataManager.Instance.MineralData.GetAllItems()
                        .Where(item => item.itemType == MINERAL_TYPE.Ingot).Where(item => item.id < 120).ToList();
                }
                else
                {
                    ingots = DataManager.Instance.MineralData.GetAllItems().Where(item => item.itemType == MINERAL_TYPE.Ingot).ToList();
                }
                PoolManager.Instance.GetFromPool(ingots[Random.Range(0, ingots.Count)].id, monsterPos);
                break;
        }

    }

    /// <summary>
    /// 확률에 따라서 해당 아이템을 줄 건지를 결정
    /// </summary>
    private bool CanDrop(float dropProbability)
    {
        int rand = Random.Range(1, 101);
        return dropProbability >= rand;
    }

    /// <summary>
    /// 최소 수량과 최대 수량 사이에서 랜덤하게 수량 반환
    /// </summary>
    private int GetRandomQuantity(int minQuantity, int maxQuantity)
    {
        return Random.Range(minQuantity, maxQuantity + 1);
    }
}
