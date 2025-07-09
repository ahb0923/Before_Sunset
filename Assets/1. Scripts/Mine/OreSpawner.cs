using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreSpawner : ResourceSpawner<OreData>
{
    private void Awake()
    {
        GetId = data => data.id;
        GetSpawnStage = data => data.spawnStage;
        GetProbability = data => data.spawnProbability;

        prefabFolder = "Prefabs/Ore";
        prefabPrefix = "Ore";
    }
}