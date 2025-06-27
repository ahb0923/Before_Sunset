using UnityEngine;

public class Core : MonoBehaviour //IDamagable 필요
{
    [SerializeField] private int _size;
    public int Size => _size;
    [SerializeField] private int _maxHp = 500;
    private int currHp;

    private void Awake()
    {
        SetFullHp();
    }

    public void SetFullHp()
    {
        currHp = _maxHp;
    }

    public void TakeDamage(int damage)
    {
        currHp -= damage;

        if(currHp <= 0)
        {
            currHp = 0;
            Debug.Log("게임 오버");
        }
    }
}
