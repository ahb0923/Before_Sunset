using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreController : MonoBehaviour
{
    public OreData Data { get; private set; }

    private int _currentHP;

    public void Initialize(OreData oreData)
    {
        Data = oreData;
        _currentHP = Data.hp;
    }

    public bool CanBeMined(int pickaxePower)
    {
        if (Data == null) return false;
        return pickaxePower >= Data.def;
    }

    public bool Mine(int damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}