using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateHandler : MonoBehaviour, ISaveable
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

    /// <summary>
    /// 플레이어 위치 정보 저장<br/>
    /// ※ 혹시 플레이어 스탯이 추가될 수도 있어서 여기에서 진행
    /// </summary>
    public void SaveData(GameData data)
    {
        data.playerPosition = transform.position;
    }
}
