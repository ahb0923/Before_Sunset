using System.Collections.Generic;
using UnityEngine;

public class MonsterHandler : MonoBehaviour
{
    public MonsterAI Ai { get; private set; }
    
    public Monster_SO Stat { get; private set; }
    
    private void Awake()
    {
        Ai = GetComponent<MonsterAI>();
    }

    /// <summary>
    /// 몬스터 초기화
    /// </summary>
    public void Init(Monster_SO stat, Vector3 position, Transform target, List<Node> path)
    {
        Stat = stat;
        transform.position = position;
        Ai.AddTarget(target, path);
    }
}
