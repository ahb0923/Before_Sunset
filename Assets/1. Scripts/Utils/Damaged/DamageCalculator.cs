using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    /// <summary>
    /// 방어력을 계산하여 최종 데미지를 반환합니다.<br/>
    /// 계산식: 공격력 * 안정성(0.8-1.0 랜덤값) * (1 / (방어력 + 111) * 111)<br/>
    /// 추후 계산식 수정 예정
    /// </summary>
    /// <param name="attackDamage"></param>
    /// <param name="armor"></param>
    /// <param name="ignoreDefense"></param>
    /// <returns></returns>
    public static int CalcDamage(float attackDamage, float armor, bool ignoreDefense = false, float multiplier = 1f)
    {
        //float stability = Random.Range(0.8f, 1f);
        //float defensed = 1f / (armor + 111f) * 111f;
        //float damage = attackDamage * (ignoreDefense ? stability : stability * defensed);
        float damage = attackDamage - (ignoreDefense ? 0 : armor);
        if (damage < 0)
            damage = 0;

        damage *= multiplier;

        int result = Mathf.RoundToInt(damage);
        return result;
    }
}
