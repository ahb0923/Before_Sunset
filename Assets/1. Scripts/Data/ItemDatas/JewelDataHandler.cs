using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelDataHandler : BaseDataHandler<JewelData>
{
    protected override string FileName => "JewelData_JSON.json";
    protected override int GetId(JewelData data) => data.id;
    protected override string GetName(JewelData data) => data.itemName;
}
