using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDataHandler : BaseDataHandler<EquipmentData>
{
    protected override string FileName => "EquipmentData_JSON.json";
    protected override int GetId(EquipmentData data) => data.id;
    protected override string GetName(EquipmentData data) => data.itemName;
}
