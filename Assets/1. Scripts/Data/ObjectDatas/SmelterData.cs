using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterData
{
    public int id;
    public string smelterName;
    public int hp;


    public Dictionary<string, int> buildRequirements;
    public List<int> smeltingIdList;
}
