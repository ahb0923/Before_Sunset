using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class TowerDataManager : BaseDataManager<TowerData>
{
    protected override string DataUrl => "https://script.google.com/macros/s/your-tower-sheet-id/exec";

    protected override int GetId(TowerData data) => data.id;

    protected override string GetName(TowerData data) => data.towerName;

    public IEnumerable<TowerData> GetAll() => dataIdDictionary.Values;
}
