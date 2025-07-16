using UnityEngine;

public class PlayerStatHandler : MonoBehaviour
{
    private BasePlayer _player;

    [SerializeField] private int _initialPickaxeId = 700;
    public EquipmentDatabase Pickaxe { get; private set; }

    [SerializeField] private float _moveSpeed = 2.0f;
    public float MoveSpeed => _moveSpeed;

    /// <summary>
    /// 플레이어 스탯 초기화
    /// </summary>
    public void Init(BasePlayer player)
    {
        _player = player;

        EquipPickaxe(_initialPickaxeId);
    }

    /// <summary>
    /// 플레이어 곡괭이 적용
    /// </summary>
    public void EquipPickaxe(int id)
    {
        Pickaxe = DataManager.Instance.EquipmentData.GetById(id);
        _player.Animator.SetFloat(BasePlayer.MINING, Pickaxe.speed);
        // 나중에 곡괭이 이미지가 바뀐다거나 할 수도
    }
}
