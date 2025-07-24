using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DEBUFF_TYPE
{
    Fire,
    Ice,
    Electric
}

public abstract class BaseDebuff
{
    public DEBUFF_TYPE Type { get; private set; }
    public bool IsActive { get; protected set; }

    protected BaseMonster target;
    protected Coroutine runningCoroutine;

    protected BaseDebuff(DEBUFF_TYPE type)
    {
        Type = type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public virtual void Apply(BaseMonster target)
    {
        if (IsActive) return;

        this.target = target;
        IsActive = true;
        runningCoroutine = target.StartCoroutine(EffectCoroutine());
    }

    protected abstract IEnumerator EffectCoroutine();

    /// <summary>
    /// 
    /// </summary>
    public virtual void Remove()
    {
        if (!IsActive) return;

        if (runningCoroutine != null)
        {
            target.StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        IsActive = false;
    }

}
