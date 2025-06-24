using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_STATE
{
    Invalid,
    Exploring,
    Move,
    Attack,
    Dead,
}

public class MonsterAI : StateBasedAI<MONSTER_STATE>
{
    [Header("# Enemy Stat")]
    [SerializeField] private int _hp;       // 체력
    [SerializeField] private int _damage;   // 데미지
    [SerializeField] private float _aps;    // 초당 공격 횟수
    [SerializeField] private float _speed;  // 이동 속도
    [SerializeField] private float _range;  // 공격 사거리
    [SerializeField] private int _size;     // 몬스터 사이즈

    private Transform _target;
    private List<Node> _path = new List<Node>();

    protected override MONSTER_STATE InvalidState => MONSTER_STATE.Invalid;

    protected override IEnumerator OnStart()
    {
        _target = MapManager.Instance.TestCore;
        CurState = MONSTER_STATE.Exploring;
        yield break;
    }

    protected override void DefineStates()
    {
        AddState(MONSTER_STATE.Exploring, new StateElem
        {
            Entered = () => Debug.Log("탐색 시작"),
            Doing = C_Explore,
            Exited = () => Debug.Log("탐색 종료")
        });

        AddState(MONSTER_STATE.Move, new StateElem
        {
            Entered = () => Debug.Log("이동 시작"),
            Doing = C_Move,
            Exited = () => Debug.Log("이동 종료")
        });

        AddState(MONSTER_STATE.Attack, new StateElem
        {
            Entered = () => Debug.Log("공격 시작"),
            Doing = C_Attack,
            Exited = () => Debug.Log("공격 종료")
        });

        AddState(MONSTER_STATE.Dead, new StateElem
        {
            Entered = () => Debug.Log("사망"),
        });
    }

    protected override bool IsAIEnded()
    {
        return CurState == MONSTER_STATE.Dead;
    }

    protected override bool IsTerminalState(MONSTER_STATE state)
    {
        return false;
    }

    /// <summary>
    /// 코어를 향한 가장 빠른 길 탐색 → 그 길을 막고 있는 타워를 향한 길 탐색
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_Explore()
    {
        yield return StartCoroutine(MapManager.Instance.C_FindPath(transform.position, _target.position, path => _path = path));

        if (_path == null)
        {
            Debug.LogWarning("경로를 찾지 못했습니다.");
            // 가장 빠른 루트를 방해하는 타워 찾기 로직
            // 그리고, _target을 해당 타워로 바꿔주면 됨
            CurState = MONSTER_STATE.Exploring;
            yield break;
        }

        CurState = MONSTER_STATE.Move;
    }

    /// <summary>
    /// path를 따라서 이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_Move()
    {
        while(_path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, _path[0].WorldPos, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _path[0].WorldPos) < 0.01f)
            {
                transform.position = _path[0].WorldPos;
                _path.RemoveAt(0);
            }
            yield return null;
        }

        CurState = MONSTER_STATE.Attack;
    }

    /// <summary>
    /// 초당 공격 횟수에 따른 공격
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_Attack()
    {
        yield return null;
    }

    // 오브젝트 풀링 사용 대비 test
    private void OnEnable()
    {
        if (!HasStarted)
            return;

        _target = MapManager.Instance?.TestCore;
        RunDoingState();
        TransitionTo(MONSTER_STATE.Exploring, true);
    }

    // test 용
    private void OnDisable()
    {
        CurState = MONSTER_STATE.Dead;
    }
}
