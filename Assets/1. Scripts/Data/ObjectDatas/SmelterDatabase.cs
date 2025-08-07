using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmelterDatabase
{
    public int id;
    public string smelterName;
    public string prefabName;
    public int hp;


    public Dictionary<string, int> buildRequirements;
    public List<int> smeltingIdList;
}
