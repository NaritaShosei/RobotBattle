using Cysharp.Threading.Tasks;
using SymphonyFrameWork.System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : LongRangeAttack_B
{
    [SerializeField]
    LockOn _lockOn;

    bool _isAttack;

    public bool IsAttack { get => _isAttack; set => _isAttack = value; }
    void Start()
    {
        Start_B();
    }

    void Update()
    {
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
        if (_count == _data.BulletCount) return;
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
    }
}
