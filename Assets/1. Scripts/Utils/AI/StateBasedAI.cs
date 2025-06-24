using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public abstract class StateBasedAI<T> : MonoBehaviour where T : IConvertible
{
    public T CurState
    {
        get => _curState;
        protected set => TransitionTo(value, false);
    }

    public T PrevState => _prevState;

    public bool IsInterrupted { get; set; }
    protected abstract T InvalidState { get; }

    private static readonly EqualityComparer<T> COMPARER = EqualityComparer<T>.Default;
    private readonly Dictionary<T, StateElem> _states = new Dictionary<T, StateElem>();

    private T _curState;
    private T _prevState;

    protected bool HasStarted { get; private set; } = false;
    private Coroutine _runDoingStateCoroutine;

    protected class StateElem
    {
        public Action Entered;
        public Func<IEnumerator> Doing;
        public Action Exited;
    }

    /// <summary>
    /// 현재 상태를 변경해줍니다.
    /// </summary>
    /// <param name="nextState">변경할 상태</param>
    /// <param name="force">상태를 강제적으로 전이할 것인가</param>
    protected void TransitionTo(T nextState, bool force = false)
    {
        // AI 전체가 종료된 상태면 상태변화 X   (force == true면 강제 통과)
        if (!force && IsAIEnded())
        {
            return;
        }

        //현재 상태가 종료(Terminal) 상태가 아니거나, 강제 전이라면 => 인터럽트 발생
        //IsInterrupted == true는 Doing 루프를 중단시키는 신호
        IsInterrupted = (!IsTerminalState(_curState) || force);

        //현재 상태가 Terminal 상태면 전이를 막음   (force == true면 강제 통과)
        if (!force && IsTerminalState(_curState))
        {
            return;
        }

        // 상태 갱신
        _prevState = _curState;
        _curState = nextState;

        // 이전 상태에서 벗어날 때 처리할 로직 호출 (Exited)
        if (!COMPARER.Equals(_prevState, InvalidState))
        {
            StateElem stateElem = _states.Get(_prevState, null);
            if (stateElem != null && stateElem.Exited != null)
            {
                stateElem.Exited();
            }
        }

        // 새로이 들어가는 상태에서 처리할 로직 호출(Entered )
        if (!COMPARER.Equals(_curState, InvalidState))
        {
            StateElem stateElem = _states.Get(_curState, null);
            if (stateElem != null && stateElem.Entered != null)
            {
                stateElem.Entered();
            }
        }
    }

    /// <summary>
    /// 해당 ai에서 사용할 상태의 행동 정의
    /// </summary>
    protected abstract void DefineStates();

    /* << 예시 >> 
    protected override void DefineStates()
    {
        AddState(BUILDING_STATE.Construction, new StateElem
        {
            Entered = () => Debug.Log("건설 시작"),
            Doing = C_Construction,
            Exited = () => {
                building.SpawnArcherByTier(building.statHandler.Tier);
                Debug.Log("건설 완료");
            }
        });
        AddState(BUILDING_STATE.Attack, new StateElem
        {
            Entered = () => Debug.Log("공격 시작"),
            Doing = C_Attack,
        });

        AddState(BUILDING_STATE.Idle, new StateElem
        {
            Entered = () => Debug.Log("건물 대기 중")
        });

        AddState(BUILDING_STATE.Destroy, new StateElem
        {
            Entered = () => Debug.Log("건물 파괴"),
            Doing = C_Destroy
        });
    }
     */


    /// <summary>
    /// Awake 단계에서 구현할게 있으면 이곳에
    /// </summary>
    protected virtual void OnAwake()
    {
    }
    /// <summary>
    /// Start 단계에서 구현할게 있으면 이곳에
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator OnStart()
    {
        yield break;
    }

    protected virtual IEnumerator OnBeforeDoingState()
    {
        yield break;
    }

    protected virtual IEnumerator OnAfterDoingState()
    {
        yield break;
    }

    // IsAIEnded == true    =>   몬스터라면 죽어서 비활성화 되었거나..
    // 전체 AI의 종료 여부 판단
    protected abstract bool IsAIEnded();

    // 상태 전이 제한 조건을 걸때 사용, ex) Monster의 state == dead 라면 chase라던가 상태의 전이가 불가능하게
    protected abstract bool IsTerminalState(T state);

    private void Awake()
    {
        _curState = InvalidState;
        _prevState = InvalidState;

        DefineStates();
        OnAwake();
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(OnStart());

        HasStarted = true;
        RunDoingState();
    }

    protected void RunDoingState()
    {
        // 이미 실행중인 코루틴이 있다면 종료
        if (_runDoingStateCoroutine != null)
        {
            StopCoroutine(_runDoingStateCoroutine);
            _runDoingStateCoroutine = null;
        }

        // 코루틴 시작
        _runDoingStateCoroutine = StartCoroutine(C_RunDoingState());
    }

    private IEnumerator C_RunDoingState()
    {
        // 종료된 (ex 몬스터라면 사망처리된)AI라면 작동X
        while (!IsAIEnded())
        {
            IsInterrupted = false;
            yield return StartCoroutine(OnBeforeDoingState());

            StateElem state = _states.Get(CurState, null);
            if (state != null)
            {
                if (state.Doing == null)
                {
                    while (!IsInterrupted)
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return StartCoroutine(state.Doing());
                }
            }

            yield return StartCoroutine(OnAfterDoingState());
            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// 상태를 추가합니다.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="stateElem"></param>
    protected void AddState(T state, StateElem stateElem)
    {
        _states.Add(state, stateElem);
    }
}
