using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDataHandler : BaseDataHandler<UpgradeDatabase>
{
    protected override string FileName => "UpgradeData_JSON.json";
    protected override int GetId(UpgradeDatabase data) => data.id;
    protected override string GetName(UpgradeDatabase data) => data.upgradeName;
}
