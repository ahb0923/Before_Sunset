using UnityEngine;

// 나중에 플레이어 스탯 핸들러로 변경 예정(이름)
// 말그대로 플레이어 스탯을 다루는 스크립트
public class PlayerStateHandler : MonoBehaviour
{
    public bool IsInMiningArea { get; private set; } = false;

    public void EnterMiningArea()
    {
        IsInMiningArea = true;
    }

    public void ExitMiningArea()
    {
        IsInMiningArea = false;
    }
}
