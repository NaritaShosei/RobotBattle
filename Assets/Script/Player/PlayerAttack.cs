using Cysharp.Threading.Tasks;
using SymphonyFrameWork.System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : LongRangeAttack_B
{
    [SerializeField]
    LockOn _lockOn;

    InputBuffer _input;
    void Start()
    {
        Start_B();
    }

    protected override void OnStart()
    {
        _input = ServiceLocator.GetInstance<InputBuffer>();

        base.OnStart();
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

                    var enemy = _lockOn.GetTarget();

                    bullet.SetDirection(transform.forward);
                    bullet.SetTarget(enemy);
                    bullet.gameObject.SetActive(true);
                    _count--;
                }
            }
        }
    }

    void Attack1(InputAction.CallbackContext context)
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
    async UniTaskVoid Reload()
    {
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
    }
    void OnReturnPool(Bullet_B bullet)
    {
        _bulletPool.Enqueue(bullet);
    }
    private void OnDisable()
    {
        _input.Attack1Action.started -= Attack1;
        _input.Attack1Action.canceled -= Attack1;
    }
}
