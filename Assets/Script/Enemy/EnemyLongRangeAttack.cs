using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyLongRangeAttack : LongRangeAttack_B
{
    [SerializeField]
    void Start()
    {
        Start_B();
    }

    void Update()
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
                    bullet.SetTarget(enemy);
                    bullet.gameObject.SetActive(true);
                    _count--;
                }
            }
        }
    }
    async UniTaskVoid Reload()
    {
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
    }
}
