using System;

public class UIManager : MonoSingleton<UIManager>
{
    public AskPopUpUI AskPopUpUI { get; private set; }
    public GameTimeUI GameTimeUI { get; private set; }
    public CraftArea CraftArea { get; private set; }
    public CraftMaterialArea CraftMaterialArea { get; private set; }
    public SaveLoadUI SaveLoadUI { get; private set; }
    public BattleUI BattleUI { get; private set; }
    public TowerUpgradeUI TowerUpgradeUI { get; private set; }
    public DismantleUI DismantleUI { get; private set; }
    public RecallUI RecallUI { get; private set; }
    public SmelterUI SmelterUI { get; private set; }
    public UpgradeUI UpgradeUI { get; private set; }
    public EssenceUI EssenceUI { get; private set; }
    public TutorialSkip TutorialSkipButton { get; private set; }
    public QuestUI QuestUI { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        AskPopUpUI = Helper_Component.FindChildComponent<AskPopUpUI>(this.transform, "AskPopUpUI");
        GameTimeUI = Helper_Component.FindChildComponent<GameTimeUI>(this.transform, "GameTimeUI");
        CraftArea = Helper_Component.FindChildComponent<CraftArea>(this.transform, "CraftArea");
        CraftMaterialArea = Helper_Component.FindChildComponent<CraftMaterialArea>(this.transform, "CraftMaterialArea");
        SaveLoadUI = Helper_Component.FindChildComponent<SaveLoadUI>(this.transform, "SaveLoadBG");
        BattleUI = Helper_Component.FindChildComponent<BattleUI>(this.transform, "BattleUI");
        TowerUpgradeUI = Helper_Component.FindChildComponent<TowerUpgradeUI>(this.transform, "TowerUpgradeUI");
        DismantleUI = Helper_Component.FindChildComponent<DismantleUI>(this.transform, "DismantleUI");
        RecallUI = Helper_Component.FindChildComponent<RecallUI>(this.transform, "RecallUI");
        SmelterUI= Helper_Component.FindChildComponent<SmelterUI>(this.transform, "SmelterUI");
        UpgradeUI = Helper_Component.FindChildComponent<UpgradeUI>(this.transform, "UpgradeUI");
        EssenceUI = Helper_Component.FindChildComponent<EssenceUI>(this.transform, "EssenceUI");
        TutorialSkipButton = Helper_Component.FindChildComponent<TutorialSkip>(transform, "TurorialSkipButton");
        QuestUI = Helper_Component.FindChildComponent<QuestUI>(transform, "QuestUI");
    }

    private void Start()
    {
        CraftArea.gameObject.SetActive(false);
        CraftMaterialArea.gameObject.SetActive(false);
        
        TutorialSkipButton.gameObject.SetActive(GameManager.Instance.IsTutorial);
        QuestUI.gameObject.SetActive(GameManager.Instance.IsTutorial);
    }

    private void Update()
    {
        CraftArea.ToggleCraftArea();
    }
}