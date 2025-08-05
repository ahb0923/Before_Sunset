using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] public bool GOD_MODE = false;
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
    /// 튜토리얼인지 설정
    /// </summary>
    public void SetTutorial(bool isTutorial)
    {
        IsTutorial = isTutorial;
    }

    /// <summary>
    /// 엔딩 씬으로 이동
    /// </summary>
    public void GoToEndScene()
    {
        SceneManager.LoadScene("EndingScene");
    }
}
