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
    public int DebuffId => debuffId;

    [SerializeField] private DEBUFF_TYPE debuffType;
    public DEBUFF_TYPE DebuffType => debuffType;

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
    /// 디버프 적용 시도
    /// </summary>
    public virtual void Apply(BaseMonster target)
    {
        if (IsActive) return;

        this.target = target;

        // 동일 타입의 디버프가 이미 있다면 제거 후 교체
        target.RegisterDebuff(this);

        IsActive = true;
        runningCoroutine = target.StartCoroutine(C_Effect());
    }

    /// <summary>
    /// 디버프 효과 코루틴
    /// </summary>
    protected abstract IEnumerator C_Effect();

    /// <summary>
    /// 디버프 종료 처리
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
        target.UnregisterDebuff(this); // 🔄 등록 해제
    }

    public int GetId() => debuffId;

    public void OnInstantiate()
    {
        InitSetting();
    }

    public void OnGetFromPool()
    {
        IsActive = false;
        runningCoroutine = null;
    }

    public void OnReturnToPool()
    {
        // 풀로 갈 때 강제 종료
        Remove();
        target = null;
    }
}