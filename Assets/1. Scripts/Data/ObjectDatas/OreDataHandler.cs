using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDataHandler : BaseDataHandler<OreDatabase>
{
    protected override string FileName => "OreData_JSON.json";
    protected override int GetId(OreDatabase data) => data.id;
    protected override string GetName(OreDatabase data) => data.itemName;
}
