using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
    bool IsInteractable(Vector3 playerPos, float range, BoxCollider2D playerCollider);
    public int GetObejctSize();
}
