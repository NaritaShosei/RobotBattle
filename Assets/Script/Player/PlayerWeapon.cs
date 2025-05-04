using Cysharp.Threading.Tasks;
using SymphonyFrameWork.System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : LongRangeAttack_B
{
    [SerializeField]
    LockOn _lockOn;
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

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _isAttacked = true;
            if (_count <= 0)
            {
                Reload().Forget();
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isAttacked = false;
        }
    }
    public async UniTaskVoid Reload()
    {
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
    }
}
