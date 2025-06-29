﻿using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyLongRangeAttack : LongRangeAttack_B
{
    IEnemy _enemy;
    bool _isAttacked;
    void Start()
    {
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
    void Attack(PlayerController player)
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
        if (_enemy != null)
        {
            _enemy.RemoveOnAttackEvent(Attack);
        }
    }
}
