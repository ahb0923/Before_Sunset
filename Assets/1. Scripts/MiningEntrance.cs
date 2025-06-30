using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningEntrance : MonoBehaviour, IInteractable
{
    private PlayerStateHandler _playerState;

    private void Start()
    {
        // 플레이어 오브젝트 찾기 (예: 태그 "Player"로 찾기)
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerState = player.GetComponent<PlayerStateHandler>();
    }

    public bool IsInteractable(Vector3 playerPos, float range)
    {
        return Vector3.Distance(playerPos, transform.position) <= range;
    }

    public void Interact()
    {
        if (_playerState != null)
        {
            _playerState.IsInMiningArea = true;  // 광산 상태로 변경
            Debug.Log("광산 입장: IsInMiningArea = true");
        }
    }
}
