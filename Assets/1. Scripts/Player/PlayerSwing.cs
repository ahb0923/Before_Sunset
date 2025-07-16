using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwing : MonoBehaviour
{
    private BasePlayer _player;
    private EquipmentDatabase _equippedPickaxe;

    [SerializeField] private LayerMask _miningLayer;
    private Vector2 _clickDir;

    public bool IsSwing { get; private set; }

    /// <summary>
    /// 플레이어 스윙 초기화
    /// </summary>
    public void Init(BasePlayer player)
    {
        _player = player;
        _equippedPickaxe = player.Stat.Pickaxe;

        IsSwing = false;
    }

    /// <summary>
    /// 스윙 애니메이션 실행
    /// </summary>
    public void Swing()
    {
        IsSwing = true;

        // 클릭 월드 포지션 구하기
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        clickPos.z = 0f;

        // 플레이어 기준으로 클릭의 방향 벡터 구하기
        _clickDir = (clickPos - transform.position).normalized;
        if (Mathf.Abs(_clickDir.x) > Mathf.Abs(_clickDir.y))
            _clickDir = Vector2.right * (_clickDir.x > 0 ? 1 : -1);
        else
            _clickDir = Vector2.up * (_clickDir.y > 0 ? 1 : -1);

        _player.Animator.SetFloat(BasePlayer.X, _clickDir.x);
        _player.Animator.SetFloat(BasePlayer.Y, _clickDir.y);
        _player.Animator.SetTrigger(BasePlayer.SWING);

        // 디버그 코드
        Debug.DrawRay(transform.position, _clickDir * _equippedPickaxe.range, Color.red, 1f);
    }

    /// <summary>
    /// 채광 시도
    /// </summary>
    public void TryMineOre()
    {
        if (_equippedPickaxe == null) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _clickDir, _equippedPickaxe.range, _miningLayer);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<OreController>(out var ore))
            {
                if (ore.CanBeMined(_equippedPickaxe.crushingForce))
                {
                    bool destroyed = ore.Mine(_equippedPickaxe.damage);
                    Debug.Log(destroyed ? "광석이 파괴됨!" : $"광석에 {_equippedPickaxe.damage}의 데미지 입힘");
                }
                else
                {
                    Debug.Log("곡괭이 파워 부족!");
                }
            }
            else if(hit.collider.TryGetComponent<JewelController>(out var jewel))
            {
                jewel.OnMined();
                Debug.Log("쥬얼 파괴됨!");
            }
        }
        else
        {
            Debug.Log("채광 범위 내에 광석/보석 없음");
        }

        IsSwing = false;
    }
}
