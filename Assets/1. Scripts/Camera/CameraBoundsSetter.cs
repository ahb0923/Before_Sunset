using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class CameraBoundsSetter : MonoBehaviour
{
    public Tilemap tilemap;
    public CinemachineConfiner2D confiner;

    void Start()
    {
        StartCoroutine(SetupBoundsDelayed());
    }

    IEnumerator SetupBoundsDelayed()
    {
        yield return null; // 1프레임 기다려야 Collider가 완성됨

        CompositeCollider2D composite = tilemap.GetComponent<CompositeCollider2D>();
        if (composite != null)
        {
            confiner.m_BoundingShape2D = null;
            yield return null; // 한 프레임 더 기다리기 안정성 ↑
            confiner.m_BoundingShape2D = composite;

            Debug.Log("카메라 경계 설정 완료");
        }
        else
        {
            Debug.LogWarning("CompositeCollider2D가 Tilemap에 없습니다!");
        }
    }
}
