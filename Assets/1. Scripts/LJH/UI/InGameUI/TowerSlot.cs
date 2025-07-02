using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public int Index { get; private set; }
    
    public void InitIndex(int index)
    {
        Index = index;
    }
}
