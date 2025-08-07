using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player != null)
            transform.position = player.position;
    }
}
