using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public enum GIZMO_TYPE
{
    Sphere,
    WireBox,
    WireSphere,
    Ray,
}
public enum RAYDIRECTION
{
    Forward,
    Up,
    Right
}

public class DrawGizmos : MonoBehaviour
{
    public Color color = Color.red;
    public float radius = 0.1f;

    public GIZMO_TYPE currType = GIZMO_TYPE.Sphere;
    public RAYDIRECTION rayDirection = RAYDIRECTION.Forward;
    public Vector3 size = Vector3.one;


    public void OnDrawGizmos()
    {
        if (!enabled) return;

        Gizmos.color = color;
        Vector3 dir = rayDirection switch
        {
            RAYDIRECTION.Forward => transform.forward,
            RAYDIRECTION.Up => transform.up,
            RAYDIRECTION.Right => transform.right,
            _ => transform.forward
        };

        switch (currType)
        {
            case GIZMO_TYPE.Sphere:
                Gizmos.DrawSphere(transform.position, radius);
                break;
            case GIZMO_TYPE.WireBox:
                Gizmos.DrawWireCube(transform.position, size);
                break;
            case GIZMO_TYPE.WireSphere:
                Gizmos.DrawWireSphere(transform.position, radius);
                break;
            case GIZMO_TYPE.Ray:
                Gizmos.DrawRay(transform.position, dir * radius);
                break;

        }
    }
}
