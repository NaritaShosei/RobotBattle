using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttack_B : WeaponBase
{

    [SerializeField] protected Bullet_B _bullet;

    [SerializeField] protected Transform _muzzle;

    [SerializeField] protected LayerMask _layer;

    protected BulletManager _bulletManager;
    protected float _time;
    protected int _count;

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    protected void Start_B()
    {
        OnStart();
    }

    void OnStart()
    {
        _bulletManager = ServiceLocator.Get<BulletManager>();
        _time = Time.time;
        _count = _data.BulletCount;
        _bulletManager.SetPool(this, _bullet, _count, _layer);
    }
}
