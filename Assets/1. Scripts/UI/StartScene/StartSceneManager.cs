using UnityEngine;

public class StartSceneManager : MonoSingleton<StartSceneManager>
{
    public StartSceneUI StartSceneUI { get; private set; }
    public StartSceneAnimation StartSceneAnimation { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StartSceneUI = GameObject.Find("Canvas").GetComponent<StartSceneUI>();
        StartSceneAnimation = GameObject.Find("StartSceneAnimation").GetComponent<StartSceneAnimation>();
    }
}