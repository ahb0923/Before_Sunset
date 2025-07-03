using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] public GameObject testBuildSlot;
    public GameTimeUI GameTimeUI { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GameTimeUI = GetComponentInChildren<GameTimeUI>();
    }
}
