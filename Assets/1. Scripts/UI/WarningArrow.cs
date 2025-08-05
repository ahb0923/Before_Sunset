using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WARNING_DIRECTION
{
    Up,
    Down, 
    Left, 
    Right
}
public class WarningArrow : MonoBehaviour
{
    [SerializeField] private Transform UpSide;
    [SerializeField] private Transform DownSide;
    [SerializeField] private Transform LeftSide;
    [SerializeField] private Transform RightSide;

    private void Reset()
    {
        UpSide = Helper_Component.FindChildByName(transform, "UpSide");
        DownSide = Helper_Component.FindChildByName(transform, "DownSide");
        LeftSide = Helper_Component.FindChildByName(transform, "LeftSide");
        RightSide = Helper_Component.FindChildByName(transform, "RightSide");
    }

    private void Start()
    {
        OffAllWarning();
    }

    /// <summary>
    /// 특정방향 워닝 사인 On
    /// </summary>
    /// <param name="dir">나오는 방향</param>
    /// <param name="trigger"></param>
    public void SetWarning(WARNING_DIRECTION dir, bool trigger)
    {
        switch (dir)
        {
            case WARNING_DIRECTION.Up:
                UpSide.gameObject.SetActive(trigger);
                break;
            case WARNING_DIRECTION.Down:
                DownSide.gameObject.SetActive(trigger);
                break;
            case WARNING_DIRECTION.Left:
                LeftSide.gameObject.SetActive(trigger);
                break;
            case WARNING_DIRECTION.Right:
                RightSide.gameObject.SetActive(trigger);
                break;
        }
    }

    /// <summary> 모든 워닝사인 OFF </summary>
    public void OffAllWarning()
    {
        UpSide.gameObject.SetActive(false);
        DownSide.gameObject.SetActive(false);
        LeftSide.gameObject.SetActive(false);
        RightSide.gameObject.SetActive(false);
    }

    [ContextMenu("up")]
    public void Upside()
    {
        UpSide.gameObject.SetActive(true);
    }
    [ContextMenu("down")]
    public void Downside()
    {
        DownSide.gameObject.SetActive(true);
    }
    [ContextMenu("left")]
    public void Leftside()
    {
        LeftSide.gameObject.SetActive(true);
        
    }
    [ContextMenu("right")]
    public void Rightside()
    {
        RightSide.gameObject.SetActive(true);
    }
    [ContextMenu("Off")]
    public void Off()
    {
        OffAllWarning();
    }
}
