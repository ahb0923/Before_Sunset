using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedSystem : PlainSingleton<DamagedSystem>
{
    private DamagedProcesser _damagedProcesser = new DamagedProcesser();

    public void Send(Damaged damaged)
    {
        _damagedProcesser.Add(damaged);
        _damagedProcesser.Update();
    }
}
