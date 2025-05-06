using SymphonyFrameWork.System;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttack_B : MonoBehaviour
{
    [SerializeField] protected AttackData _data;

    [SerializeField] protected Bullet_B _bullet;

    [SerializeField] protected Transform _muzzle;

    [SerializeField] protected Transform _bulletParent;

    [SerializeField] protected LayerMask _layer;
    protected Queue<Bullet_B> _bulletPool = new();
    protected float _time;
    protected int _count;

    protected void Start_B()
    {
        OnStart();
    }

    void OnStart()
    {
        _time = Time.time;
        _count = _data.BulletCount;
        for (int i = 0; i < _data.BulletCount * 1.5f; i++)
        {
            var bullet = Instantiate(_bullet, _bulletParent);
            bullet.ReturnPoolEvent = OnReturnPool;
            bullet.gameObject.SetActive(false);
            _bulletPool.Enqueue(bullet);
            bullet.gameObject.layer = Mathf.RoundToInt(Mathf.Log(_layer.value, 2));
        }
    }
    void OnReturnPool(Bullet_B bullet)
    {
        _bulletPool.Enqueue(bullet);
    }
}
