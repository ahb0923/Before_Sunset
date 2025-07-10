using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPassZone : MonoBehaviour
{
    [Tooltip("토글할 오브젝트들")]
    [SerializeField] private GameObject[] targetObjects;

    [Tooltip("플레이어 태그")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(!obj.activeSelf);
                }
            }
        }
    }
}
