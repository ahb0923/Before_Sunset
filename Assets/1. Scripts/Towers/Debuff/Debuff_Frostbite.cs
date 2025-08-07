using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff_Frostbite : BaseDebuff
{
    private float originalMoveSpeed;

    protected override IEnumerator C_Effect()
    {
        var data = DataManager.Instance.DebuffData.GetById(DebuffId);
        float slowRate = (data.effectValue / 100f);
        //float duration = data.duration;
        float duration = 5;

        // 대상 몬스터의 이동속도를 감소
        var statHandler = target.Stat;
        originalMoveSpeed = statHandler.Speed;

        float slowFactor = slowRate;
        statHandler.SetSpeed(originalMoveSpeed * slowFactor);

        // 디버프 지속시간만큼 대기
        yield return new WaitForSeconds(duration);

        // 원래 속도로 복원
        statHandler.SetSpeed();

        // 디버프 제거 및 풀로 반환
        Remove();
        PoolManager.Instance.ReturnToPool(DebuffId, gameObject);
    }
}
