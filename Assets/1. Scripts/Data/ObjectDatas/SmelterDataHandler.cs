using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterDataHandler : BaseDataHandler<SmelterDatabase>
{
    protected override string FileName => "SmelterData_JSON.json";

    protected override int GetId(SmelterDatabase data) => data.id;

    protected override string GetName(SmelterDatabase data) => data.smelterName;
}
