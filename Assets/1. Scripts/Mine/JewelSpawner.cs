using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelSpawner : ResourceSpawner<JewelDatabase>
{
    private void Awake()
    {
        GetId = data => data.id;
        GetSpawnStage = data => 1;
        GetProbability = data => data.spawnProbability;
    }
}
