using UnityEngine;

public enum QUEST_TYPE
{
    MoveToMine,     // 광산으로 이동
    GainItem,       // 아이템 획득
    GoToBase,       // 기지 귀환
    PlaceBuilding,  // 빌딩 설치
    UpgradeTower,   // 타워 업그레이드
    TimeSkip,       // 시간 스킵
    KillMonster,    // 몬스터 사냥
    DestroyBuilding,// 빌딩 해체
    UpgradeCore,    // 코어 업그레이드
}

[System.Serializable]
public class Quest
{
    [field: SerializeField] public QUEST_TYPE Type { get; private set; }

    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }

    [field: SerializeField] public int ClearAmount { get; private set; }
    private int curAmount;
    public bool isClear => ClearAmount <= curAmount;

    public void AddAmount(int amount)
    {
        curAmount += amount;
    }
}
