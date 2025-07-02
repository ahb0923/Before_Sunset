using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningHandler : MonoBehaviour, IInteractable
{
    private PlayerStateHandler _playerState;

    [SerializeField] private bool isEntering = true;

    private Collider2D _collider;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerState = player.GetComponent<PlayerStateHandler>();
        }

        _collider = GetComponent<Collider2D>();
    }

    public bool IsInteractable(Vector3 playerPos, float range, CircleCollider2D playerCollider)
    {
        if (_collider == null || playerCollider == null)
            return false;

        Vector2 playerPos2D = new Vector2(playerPos.x, playerPos.y);
        Vector2 closestPoint = _collider.ClosestPoint(playerPos2D);
        float distance = Vector2.Distance(playerPos2D, closestPoint);

        float playerRadius = playerCollider.radius * Mathf.Max(playerCollider.transform.lossyScale.x, playerCollider.transform.lossyScale.y);
        distance -= playerRadius;

        return distance <= range;
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
