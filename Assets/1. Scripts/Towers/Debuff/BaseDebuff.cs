using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DEBUFF_TYPE
{
    Burn,
    Frostbite,
    Electricshock
}

public abstract class BaseDebuff : MonoBehaviour, IPoolable
{
    [SerializeField] private int debuffId;
    public int DebuffId { get { return debuffId; } }

    [SerializeField] private DEBUFF_TYPE debuffType;
    public DEBUFF_TYPE DebuffType { get { return debuffType; } }

    public bool IsActive { get; protected set; }

    protected BaseMonster target;
    protected Coroutine runningCoroutine;

    public float value;
    public float duration;

    public void InitSetting()
    {
        var data = DataManager.Instance.DebuffData.GetById(debuffId);

        value = data.effectValue;
        duration = data.duration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public virtual void Apply(BaseMonster target)
    {
        if (IsActive) return;

        this.target = target;
        target.isDebuffed = true;
        IsActive = true;
        runningCoroutine = target.StartCoroutine(C_Effect());
    }

    protected abstract IEnumerator C_Effect();


    public virtual void Remove()
    {
        if (!IsActive) return;

        if (runningCoroutine != null)
        {
            target.StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        target.isDebuffed = false;
        IsActive = false;
    }

    public int GetId()
    {
        return debuffId;
    }

    public void OnInstantiate()
    {
        InitSetting();
    }

    public void OnGetFromPool()
    {
    }

    public void OnReturnToPool()
    {
        target = null;
    }
}
