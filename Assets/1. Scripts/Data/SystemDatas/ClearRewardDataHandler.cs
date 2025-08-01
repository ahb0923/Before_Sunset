using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRewardDataHandler : BaseDataHandler<ClearRewardDatabase>
{
    protected override string FileName => "ClearRewardData_JSON.json";
    protected override int GetId(ClearRewardDatabase data) => data.stageId;
    protected override string GetName(ClearRewardDatabase data) => null;
}
