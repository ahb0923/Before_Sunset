using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MAP_TYPE
{
    Base,
    MineSmall,
    MineLarge,
    MineRare
}
public class MapDatabase
{
    public int id;
    public string mapName;
    public string prefabName;
    public MAP_TYPE mapType;
}
