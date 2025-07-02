using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateHandler : MonoBehaviour
{
    public bool IsInMiningArea { get; private set; } = false;

    public void EnterMiningArea()
    {
        IsInMiningArea = true;
    }

    public void ExitMiningArea()
    {
        IsInMiningArea = false;
    }
}
