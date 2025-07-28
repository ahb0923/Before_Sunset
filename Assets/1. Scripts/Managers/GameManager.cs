using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public bool IsTutorial {  get; private set; }
    public float InitProgress { get; private set; }

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
        InitProgress = 0f;
        
        await Task.Delay(200);
        InitProgress = 0.2f;
        
        await DataManager.Instance.InitCheck();
        InitProgress = 0.8f;
        PoolManager.Instance.InitPool();
        
        await Task.Delay(200);
        InitProgress = 1f;
    }

    /// <summary>
    /// 새로운 게임 시작
    /// </summary>
    public void StartNewGame()
    {
        IsTutorial = false;
        LoadingSceneController.LoadScene("MainScene");
    }

    /// <summary>
    /// 세이브된 게임 시작
    /// </summary>
    public void StartSavedGame()
    {
        IsTutorial = false;

        // 세이브된 데이터 기반 게임 시작 로직

        LoadingSceneController.LoadScene("MainScene");
    }

    /// <summary>
    /// 튜토리얼 시작
    /// </summary>
    public void StartTutorial()
    {
        IsTutorial = true;
        LoadingSceneController.LoadScene("TutorialScene");
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
