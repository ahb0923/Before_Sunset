using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelSpawner : ResourceSpawner<JewelDatabase>
{
    private void Awake()
    {
        GetId = data => data.id;
        GetProbability = data => data.spawnProbability;
    }
}
