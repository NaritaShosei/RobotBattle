using Cysharp.Threading.Tasks;
using SymphonyFrameWork.System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    AttackData _data;

    [SerializeField]
    Bullet_B _attack1Bullet;

    [SerializeField]
    Transform _attack1Muzzle;

    [SerializeField]
    Transform _bulletParent;
    Queue<Bullet_B> _attack1BulletPool = new();

    InputBuffer _input;

    bool _isAttacked1;
    /// <summary>
    /// 残弾数
    /// </summary>
    int _count;
    float _time;
    void Start()
    {
        _time = Time.time;
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _input.Attack1Action.started += Attack1;
        _input.Attack1Action.canceled += Attack1;
        _count = _data.BulletCount;
        for (int i = 0; i < _data.BulletCount; i++)
        {
            var bullet = Instantiate(_attack1Bullet, _bulletParent);
            bullet.ReturnPoolEvent = OnReturnPool;
            bullet.gameObject.SetActive(false);
            bullet.SetAttackValue(_data.AttackPower);
            _attack1BulletPool.Enqueue(bullet);
            bullet.gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
        }
    }

    void Update()
    {
        if (_isAttacked1)
        {
            if (_attack1BulletPool.Count != 0 && _count != 0)
            {
                float rate = 1 / _data.AttackRate;
                if (Time.time > _time + rate)
                {
                    _time = Time.time;
                    var bullet = _attack1BulletPool.Dequeue();
                    bullet.SetPosition(_attack1Muzzle.position);
                    bullet.SetDirection(transform.forward);
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
            _isAttacked1 = true;
            if (_count <= 0)
            {
                Reload().Forget();
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isAttacked1 = false;
        }
    }
    async UniTaskVoid Reload()
    {
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
    }
    void OnReturnPool(Bullet_B bullet)
    {
        _attack1BulletPool.Enqueue(bullet);
    }
    private void OnDisable()
    {
        _input.Attack1Action.started -= Attack1;
        _input.Attack1Action.canceled -= Attack1;
    }
}
