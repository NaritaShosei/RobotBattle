using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttack_B : Weapon_B
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

    public override bool IKEnable() { return true; }

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
        _count = _data.AttackCapacity;
        var bullets = _bulletManager.SetPool(this, _bullet, _count, _layer);
        Array.ForEach(bullets, b => b.Initialize(_data));
    }

    public override bool CanAttackAnimPlay()
    {
        // 攻撃可能であればアニメーション再生可能
        if (_count <= 0)
        {
            Reload();
            return false;
        }

        return true;
    }
}
