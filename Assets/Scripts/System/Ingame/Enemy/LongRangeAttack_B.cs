using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
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

    public override int Count => _count;

    public override void Attack() { }

    public override Vector3 GetTargetPos()
    {
        return default;
    }

    public override void IKEnable(AimIK ik, bool enable) { }

    public override async void Reload()
    {
        await OnReload();
    }

    protected virtual async UniTask OnReload() { }

    public override void SetAttack(bool value)
    {

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
