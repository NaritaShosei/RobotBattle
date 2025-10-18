using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyLongRangeAttack : LongRangeAttack_B
{
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private AnimationController _animController;
    IEnemy _enemy;
    bool _isAttacked;

    private void Start()
    {
        Initialize(_weaponData);
        Start_B();
        _isAttacked = true;
    }

    private void OnEnable()
    {
        if (TryGetComponent(out _enemy))
        {
            _enemy.AddOnAttackEvent(Attack);
        }
    }
    private void Attack(PlayerController player)
    {
        if (_isAttacked)
        {
            if (_bulletManager.IsPoolCount(this) && _count != 0)
            {
                float rate = 1 / _data.AttackRate;
                if (Time.time > _time + rate)
                {
                    _time = Time.time;
                    var bullet = _bulletManager.GetBullet(this);
                    bullet.gameObject.SetActive(true);
                    bullet.SetPosition(_muzzle.position);

                    bullet.SetTarget(player);
                    _count--;
                }
            }

            if (_count <= 0)
            {
                _isAttacked = false;
                Reload();
            }
        }
    }

    protected override async UniTask OnReload()
    {
        await UniTask.Delay((int)(_data.CoolTime * 1000));
        _count = _data.AttackCapacity;
        _isAttacked = true;
    }
    private void OnDisable()
    {
        if (_enemy != null)
        {
            _enemy.RemoveOnAttackEvent(Attack);
        }
    }
}
