using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    private Collider2D _collider;
    private float _interactionRange = 5.0f;

    [Header("상호작용 토글 대상")]
    public GameObject targetToToggle;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Interact()
    {
        Transform lighting = transform.Find("Lighting");
        if (lighting != null)
            lighting.gameObject.SetActive(!lighting.gameObject.activeSelf);

        if (targetToToggle != null)
            targetToToggle.SetActive(!targetToToggle.activeSelf);
    }

    /// <summary>
    /// 플레이어 위치, 플레이어 콜라이더, 오브젝트 콜라이더 모두 외곽 거리 고려
    /// </summary>
    public bool IsInteractable(Vector3 playerPos, float range, BoxCollider2D playerCollider)
    {
        if (_collider == null || playerCollider == null) return false;

        Vector2 playerPos2D = new Vector2(playerPos.x, playerPos.y);
        Vector2 closestPoint = _collider.ClosestPoint(playerPos2D);
        float centerToEdge = Vector2.Distance(playerPos2D, closestPoint);

        float playerRadius = playerCollider.size.magnitude * 0.5f * Mathf.Max(playerCollider.transform.lossyScale.x, playerCollider.transform.lossyScale.y);
        float edgeToEdgeDistance = Mathf.Max(0f, centerToEdge - playerRadius);

        return edgeToEdgeDistance <= _interactionRange;
    }
}