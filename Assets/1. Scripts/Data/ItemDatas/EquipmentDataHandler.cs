using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDataHandler : BaseDataHandler<EquipmentDatabase>
{
    protected override string FileName => "EquipmentData_JSON.json";
    protected override int GetId(EquipmentDatabase data) => data.id;
    protected override string GetName(EquipmentDatabase data) => data.itemName;
}
