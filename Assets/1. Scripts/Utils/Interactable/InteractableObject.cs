using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public float interactionRange = 5.0f;

    public void Interact()
    {
        Debug.Log($"{gameObject.name}과 상호작용");
    }

    public bool IsInteractable(Vector3 playerPos, float range)
    {
        float distance = Vector3.Distance(transform.position, playerPos);
        return distance <= range;
    }
}

