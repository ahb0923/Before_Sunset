using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class MineralDataHandler : BaseDataHandler<MineralDatabase>
{
    //protected override string DataUrl => "https://script.google.com/macros/s/AKfycbz0b4dwz-nu3icZa1vauBU0EWtUa8v259evQF4EJ_MWIkLiYZHvK0LbItdWmQ3gFdcb/exec";
    protected override string FileName => "MineralData_JSON.json";
    protected override int GetId(MineralDatabase data) => data.id;
    protected override string GetName(MineralDatabase data) => data.itemName;
}