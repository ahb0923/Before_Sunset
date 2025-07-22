using UnityEngine;

public class PlayerStatHandler : MonoBehaviour
{
    private BasePlayer _player;

    [SerializeField] private int _initialPickaxeId = 700;
    public EquipmentDatabase Pickaxe => (EquipmentDatabase)InventoryManager.Instance.Inventory.Pickaxe.Data;

    [SerializeField] private float _moveSpeed = 2.0f;
    public float MoveSpeed => _moveSpeed;

    /// <summary>
    /// 플레이어 스탯 초기화
    /// </summary>
    public void Init(BasePlayer player)
    {
        _player = player;

        if (InventoryManager.Instance.Inventory.Pickaxe == null)
        {
            InventoryManager.Instance.Inventory.SetPickaxe(DataManager.Instance.EquipmentData.GetById(_initialPickaxeId));
        }
    }
}
