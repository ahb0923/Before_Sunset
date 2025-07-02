using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 광산 내 생성된 광석 오브젝트의 동작을 제어하는 클래스
/// </summary>
public class OreController : MonoBehaviour
{
    public OreData Data { get; private set; }

    private int _currentHP;

    /// <summary>
    /// 광석 데이터로 초기화 (체력 등 설정)
    /// </summary>
    public void Initialize(OreData oreData)
    {
        Data = oreData;
        _currentHP = Data.hp;
    }

    /// <summary>
    /// 채굴 가능한지 여부 반환 (곡괭이 파괴력 >= 방어력)
    /// </summary>
    public bool CanBeMined(int pickaxePower)
    {
        if (Data == null) return false;
        return pickaxePower >= Data.def;
    }

    /// <summary>
    /// 채굴 시 데미지를 입히고 파괴 여부 반환
    /// </summary>
    /// <returns>파괴되면 true</returns>
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
