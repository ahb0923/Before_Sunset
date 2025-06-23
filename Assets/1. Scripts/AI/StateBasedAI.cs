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

        _prevState = _curState;
        _curState = nextState;

        if (!COMPARER.Equals(_prevState, InvalidState))
        {
            StateElem stateElem = _states.Get(_prevState, null);
            if (stateElem != null && stateElem.Exited != null)
            {
                stateElem.Exited();
            }
        }

        if (!COMPARER.Equals(_curState, InvalidState))
        {
            StateElem stateElem = _states.Get(_curState, null);
            if (stateElem != null && stateElem.Entered != null)
            {
                stateElem.Entered();
            }
        }
    }

    protected abstract void DefineStates();

    protected virtual void OnAwake()
    {
    }

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

    protected abstract bool IsAIEnded();

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
        RunDoingState();
    }

    protected void RunDoingState()
    {
        if (_runDoingStateCoroutine != null)
        {
            StopCoroutine(_runDoingStateCoroutine);
            _runDoingStateCoroutine = null;
        }

        _runDoingStateCoroutine = StartCoroutine(C_RunDoingState());
    }

    private IEnumerator C_RunDoingState()
    {
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
