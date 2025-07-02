using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningHandler : MonoBehaviour, IInteractable
{
    private PlayerStateHandler _playerState;

    [SerializeField] private bool isEntering = true;

    private void Start()
    {
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
        if (_playerState == null)
            return;

        if (isEntering)
        {
            _playerState.EnterMiningArea();
            Debug.Log("광산 입장");
        }
        else
        {
            _playerState.ExitMiningArea();
            Debug.Log("광산 퇴장");
        }
    }
}
