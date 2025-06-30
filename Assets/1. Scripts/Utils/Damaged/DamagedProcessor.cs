using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedProcesser
{
    public event Action<Damaged> Damaged;

    private Queue<Damaged> _damagedQueue = new Queue<Damaged>();

    public void Add(Damaged damaged)
    {
        _damagedQueue.Enqueue(damaged);
    }

    public void Update()
    {
        ProcessDamagedQueue();
    }

    private void ProcessDamagedQueue()
    {
        while (_damagedQueue.Count > 0)
        {
            Damaged next = _damagedQueue.Dequeue();
            Process(next);
        }
    }

    private void Process(Damaged damaged)
    {
        if (damaged.Victim.TryGetComponent(out IDamageable victim))
        {
            victim.OnDamaged(damaged);
        }

        Damaged?.Invoke(damaged);
    }
}
