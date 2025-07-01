using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralController : MonoBehaviour
{
    public MineralData data { get; private set; }
    private int currentHP;

    /// <summary>
    /// 자원 초기화: MineralData로부터 체력/정보 설정
    /// </summary>
    public void Initialize(MineralData mineralData)
    {
        data = mineralData;
        currentHP = data.health;
    }

    /// <summary>
    /// 이 자원을 채굴할 수 있는지 검사 (곡괭이 파쇄력 >= 방어력)
    /// </summary>
    public bool CanBeMined(int pickaxePower)
    {
        if (data == null) return false;
        return pickaxePower >= data.defense;
    }

    /// <summary>
    /// 채굴 시 데미지를 입히고, 체력이 0 이하이면 파괴
    /// </summary>
    /// <returns>채굴 완료 시 true</returns>
    public bool Mine(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}

