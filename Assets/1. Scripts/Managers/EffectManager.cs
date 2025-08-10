using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoSingleton<EffectManager>
{

    public void TriggerAOE(Vector3 position, Color color, float size, bool reverse = false)
    {

        GameObject effect = PoolManager.Instance.GetFromPool(10001, position, transform);
        var pulse = effect.GetComponent<AOEPulseEffect>();
        
        if (reverse) 
            pulse.SetReverse();

        pulse.SetSize(size);
       
        pulse.color = color;
   
        if (pulse != null)
            pulse.Play();
    }

    public void MiningEffect(Transform transform)
    {
        GameObject effect = PoolManager.Instance.GetFromPool(10003, transform.position);

        effect.transform.position = transform.position;

        var mining = effect.GetComponent<Effect_Mining>();
        if (mining != null) mining.Play();
    }
}
