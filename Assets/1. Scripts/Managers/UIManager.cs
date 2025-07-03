
using Unity.VisualScripting;

public class UIManager : MonoSingleton<UIManager>
{
    public GameTimeUI GameTimeUI { get; private set; }
    public AskPopUpUI AskPopUpUI { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        GameTimeUI = GetComponentInChildren<GameTimeUI>();
        AskPopUpUI = GetComponentInChildren<AskPopUpUI>();
        
        AskPopUpUI.gameObject.SetActive(false);
    }
}