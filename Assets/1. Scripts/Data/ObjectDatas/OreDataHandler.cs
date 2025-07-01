using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDataHandler : BaseDataHandler<OreData>
{
    protected override string FileName => "OreData_JSON.json";
    protected override int GetId(OreData data) => data.id;
    protected override string GetName(OreData data) => data.itemName;
}
