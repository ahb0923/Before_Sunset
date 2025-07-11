using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelDataHandler : BaseDataHandler<JewelDatabase>
{
    protected override string FileName => "JewelData_JSON.json";
    protected override int GetId(JewelDatabase data) => data.id;
    protected override string GetName(JewelDatabase data) => data.itemName;
}
