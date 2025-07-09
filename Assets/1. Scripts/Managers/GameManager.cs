using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{

    private async void Start()
    {
        await DataManager.Instance.InitAsync();
    }
}
