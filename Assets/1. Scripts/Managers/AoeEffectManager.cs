using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeEffectManager : MonoSingleton<AoeEffectManager>
{

    public void TriggerAOE(Vector3 position, Color color, bool reverse = false)
    {

        GameObject effect = PoolManager.Instance.GetFromPool(10001, position, transform);
        var pulse = effect.GetComponent<AOEPulseEffect>();
        if (reverse) 
            pulse.SetReverse();
        pulse.color = color;
        if (pulse != null)
        {
            pulse.Play();
        }
    }
}
