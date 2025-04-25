using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyLongRangeAttack : LongRangeAttack_B
{
    [SerializeField] GameObject _enemyObj;
    IEnemy _enemy;
    void Start()
    {
        _enemyObj.TryGetComponent(out IEnemy _enemy);
        Start_B();
        _enemy.AddOnAttackEvent(Attack);
        _isAttacked = true;
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

                    bullet.SetDirection(player.GetTargetCenter().position - _muzzle.transform.position);
                    bullet.SetTarget(player);
                    bullet.gameObject.SetActive(true);
                    _count--;
                }
            }

            if (_count <= 0)
            {
                _isAttacked = false;
                Reload().Forget();
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
        _isAttacked = true;
    }
    private void OnDisable()
    {
        _enemy.RemoveOnAttackEvent(Attack);
    }
}
