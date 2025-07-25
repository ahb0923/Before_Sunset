using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    public Dictionary<string, int> BaseUpgrade { get; private set; }
    public Dictionary<string, int> FixedUpgrade { get; private set; }
    public Dictionary<string, int> VirtualUpgrade { get; private set; }
    
    public int Essence { get; private set; }
    public int EssencePiece { get; private set; }
    public int VirtualEssence { get; private set; }

    public int UsedEssence { get; private set; }
    public int VirtualUsedEssence { get; private set; }
    public int ResetCounter { get; private set; }
    public int ResetCost => 1 + ResetCounter;

    protected override void Awake()
    {
        base.Awake();
        
        InitUpgrade();
        BaseUpgrade = new Dictionary<string, int>(FixedUpgrade);
    }

    public void InitUpgrade()
    {
        if (FixedUpgrade != null)
            FixedUpgrade = null;
        
        List<UpgradeDatabase> upgradeList = DataManager.Instance.UpgradeData.GetAllItems();
        List<UpgradeDatabase> upgrades = upgradeList.Where(u => u.level == 0).ToList();
        
        FixedUpgrade = new Dictionary<string, int>();
        for (int i = 0; i < upgrades.Count; i++)
        {
            FixedUpgrade.Add(upgrades[i].upgradeName, upgrades[i].id);
        }
    }

    public void SetVirtualUpgrade()
    {
        VirtualUpgrade = new Dictionary<string, int>(FixedUpgrade);
    }

    public void DiscardVirtualUpgrade()
    {
        VirtualUpgrade = null;
    }

    public void ChangeVirtualUpgrade(string upgradeName, int id)
    {
        VirtualUpgrade[upgradeName] = id;
    }

    public void FixUpgrade()
    {
        FixedUpgrade = VirtualUpgrade;
    }
    
    public void AddEssencePiece(int amount)
    {
        EssencePiece += amount;

        if (EssencePiece >= 30)
        {
            Essence += EssencePiece / 30;
            EssencePiece %= 30;
        }
        
        UIManager.Instance.EssenceUI.Refresh();
    }

    public void SetVirtualEssence()
    {
        VirtualEssence = Essence;
        VirtualUsedEssence = UsedEssence;
    }

    public void UseVirtualEssence(int amount)
    {
        VirtualEssence -= amount;
        VirtualUsedEssence += amount;
    }

    public void FixEssence()
    {
        Essence = VirtualEssence;
        UsedEssence = VirtualUsedEssence;
    }

    public void FixResetCounter()
    {
        Essence = VirtualEssence + UsedEssence - ResetCost;
        ResetCounter++;
        UsedEssence = 0;
    }
}
