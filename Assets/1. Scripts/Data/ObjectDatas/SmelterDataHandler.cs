using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterDataHandler : BaseDataHandler<SmelterData>
{
    protected override string FileName => "SmelterData_JSON.json";

    protected override int GetId(SmelterData data) => data.id;

    protected override string GetName(SmelterData data) => data.smelterName;
}
