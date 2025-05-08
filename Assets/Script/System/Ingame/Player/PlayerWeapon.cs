using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerWeapon : LongRangeAttack_B
{
    [SerializeField]
    Sprite _weaponIcon;
    public Sprite Icon => _weaponIcon;

    public int Count => _count;

    [SerializeField]
    LockOn _lockOn;

    bool _isAttack;
    bool _isReload;
    public bool IsAttack { get => _isAttack; set => _isAttack = value; }

    void Start()
    {
        Start_B();
    }

    void Update()
    {
        if (_isReload) return;
        if (IsAttack)
        {
            if (_bulletPool.Count != 0 && _count != 0)
            {
                float rate = 1 / _data.AttackRate;
                if (Time.time > _time + rate)
                {
                    _time = Time.time;
                    var bullet = _bulletPool.Dequeue();
                    bullet.gameObject.SetActive(true);
                    bullet.SetPosition(_muzzle.position);

                    var enemy = _lockOn.GetTarget();

                    //TODO:クロスヘアの座標からRayCastを飛ばして、その方向に弾が向くようにする
                    if (enemy == null)
                    {
                        bullet.transform.forward = transform.forward;
                    }
                    bullet.SetTarget(enemy);
                    _count--;
                }
            }
        }
    }

    public void SetAttack(bool value)
    {
        IsAttack = value;
        if (value && _count <= 0)
        {
            Reload().Forget();
        }
    }
    public async UniTaskVoid Reload()
    {
        if (_isReload) return;
        if (_count == _data.BulletCount) return;
        _isReload = true;
        Debug.LogWarning("Reload" + _count);
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
        Debug.LogWarning("Reload To Complete" + _count);
        _isReload = false;
    }
}
