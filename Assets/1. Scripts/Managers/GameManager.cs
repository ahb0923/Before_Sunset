using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{

    // private async void Start()
    // {
    //     await DataManager.Instance.InitAsync();
    //
    //     PoolManager.Instance.InitPool();
    // }

    protected override void Awake()
    {
        base.Awake();
        if(Instance != null)
            DontDestroyOnLoad(this.gameObject);
    }

    public async Task InitAsync()
    {
        await DataManager.Instance.InitCheck();
        PoolManager.Instance.InitPool();
    }
}
