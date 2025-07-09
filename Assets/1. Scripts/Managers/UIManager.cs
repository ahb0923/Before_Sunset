

using System;

public class UIManager : MonoSingleton<UIManager>
{
    public AskPopUpUI AskPopUpUI { get; private set; }
    public GameTimeUI GameTimeUI { get; private set; }
    public CraftArea CraftArea { get; private set; }
    public CraftMaterialArea CraftMaterialArea { get; private set; }
    public SaveLoadUI SaveLoadUI { get; private set; }
    public BattleUI BattleUI { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        AskPopUpUI = GetComponentInChildren<AskPopUpUI>();
        GameTimeUI = Helper_Component.FindChildComponent<GameTimeUI>(this.transform, "GameTimeUI");
        CraftArea = Helper_Component.FindChildComponent<CraftArea>(this.transform, "CraftArea");
        CraftMaterialArea = Helper_Component.FindChildComponent<CraftMaterialArea>(this.transform, "CraftMaterialArea");
        SaveLoadUI = Helper_Component.FindChildComponent<SaveLoadUI>(this.transform, "SaveLoadBG");
        BattleUI = Helper_Component.FindChildComponent<BattleUI>(this.transform, "BattleUI");
        
        CraftArea.gameObject.SetActive(false);
        CraftMaterialArea.gameObject.SetActive(false);
    }

    private void Update()
    {
        CraftArea.ToggleCraftArea();
    }
}