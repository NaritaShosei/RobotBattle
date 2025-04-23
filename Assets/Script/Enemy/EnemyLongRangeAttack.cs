using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyLongRangeAttack : LongRangeAttack_B
{
    [SerializeField] Enemy_B _enemy;
    void Start()
    {
        Start_B();
        _enemy.OnAttackEvent += Attack;
    }

    void Attack(PlayerController player)
    {
        if (_isAttacked)
        {
            if (_bulletPool.Count != 0 && _count != 0)
            {
                float rate = 1 / _data.AttackRate;
                if (Time.time > _time + rate)
                {
                    _time = Time.time;
                    var bullet = _bulletPool.Dequeue();
                    bullet.SetPosition(_muzzle.position);

                    bullet.SetDirection(transform.forward);
                    bullet.SetTarget(player);
                    bullet.gameObject.SetActive(true);
                    _count--;
                }
            }
        }
    }

    void Update()
    {

    }
    async UniTaskVoid Reload()
    {
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
    }
}
