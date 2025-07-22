using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public float interactionRange = 5.0f;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Interact()
    {
        Debug.Log($"{gameObject.name}과 상호작용");

        Transform lighting = transform.Find("Lighting");
        if (lighting != null)
        {
            bool isActive = lighting.gameObject.activeSelf;
            lighting.gameObject.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("Lighting 오브젝트를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 플레이어 위치, 플레이어 콜라이더, 오브젝트 콜라이더 모두 외곽 거리 고려
    /// </summary>
    public bool IsInteractable(Vector3 playerPos, float range, CircleCollider2D playerCollider)
    {
        if (_collider == null || playerCollider == null)
            return false;

        Vector2 playerPos2D = new Vector2(playerPos.x, playerPos.y);
        Vector2 closestPointToPlayer = _collider.ClosestPoint(playerPos2D);
        float centerToEdge = Vector2.Distance(playerPos2D, closestPointToPlayer);

        // 플레이어 반지름
        float playerRadius = playerCollider.radius * Mathf.Max(playerCollider.transform.lossyScale.x, playerCollider.transform.lossyScale.y);

        // 이 오브젝트의 외곽 반지름 (서클일 경우 계산, 아니면 ClosestPoint만 사용)
        float objectRadius = 0f;
        if (_collider is CircleCollider2D circle)
        {
            objectRadius = circle.radius * Mathf.Max(circle.transform.lossyScale.x, circle.transform.lossyScale.y);
            // 원형 콜라이더는 정확히 계산 가능
            Vector2 objectCenter = (Vector2)circle.transform.position + circle.offset;
            float centerToCenter = Vector2.Distance(playerPos2D, objectCenter);
            centerToEdge = Mathf.Max(0f, centerToCenter - circle.radius); // 정확한 외곽 거리
        }

        float edgeToEdgeDistance = Mathf.Max(0f, centerToEdge - playerRadius);

        return edgeToEdgeDistance <= range;
    }
}