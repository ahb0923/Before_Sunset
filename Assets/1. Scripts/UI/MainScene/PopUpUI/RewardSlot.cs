using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] private Image _jewelImage;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    public string JewelName { get; private set; }
    public int Amount { get; private set; }
    
    private const string REWARD_ICON = "RewardIcon";
    private const string REWARD_AMOUNT = "RewardAmount";

    private void Reset()
    {
        _jewelImage = Helper_Component.FindChildComponent<Image>(this.transform, REWARD_ICON);
        _amountText = Helper_Component.FindChildComponent<TextMeshProUGUI>(this.transform, REWARD_AMOUNT);
    }

    public void Refresh(string jewelName, int amount)
    {
        this.gameObject.SetActive(true);
        JewelName = jewelName;
        Amount = amount;
        
        int id = DataManager.Instance.JewelData.GetByName(jewelName).id;
        _jewelImage.sprite = DataManager.Instance.JewelData.GetSpriteById(id);
        _amountText.text = amount.ToString("D2");
    }

    public void Clear()
    {
        this.gameObject.SetActive(false);
    }
}
