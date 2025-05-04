using Script.System.Ingame;
using System;
using UnityEngine;

public abstract class Bullet_B : MonoBehaviour
{
    protected IFightable _target;
    [SerializeField] protected BulletData _bulletData;
    public Action<Bullet_B> ReturnPoolEvent;
    protected float _timer;

    protected bool _isTimeReturned;
    protected bool _isConflictReturned;
    public float GuardBreakValue => _bulletData.GuardBreakValue;
    protected void OnEnable_B()
    {
        _isTimeReturned = false;
        _isConflictReturned = false;
    }
    protected void OnDisable_B()
    {
        if (_isTimeReturned || _isConflictReturned)
        {
            ReturnPoolEvent?.Invoke(this);
        }
    }
    abstract protected void Update();

    abstract protected void OnTriggerEnter(Collider other);
    protected abstract void Conflict(Collider other);
    protected virtual void AddDamage(float damage, IFightable fightable)
    {
        fightable.HitDamage(damage);
    }
    public virtual void SetTarget(IFightable target)
    {
        _target = target;
    }
    public virtual void SetPosition(Vector3 pos)
    {
        _timer = 0;
        transform.position = pos;
    }
    public virtual void SetDirection(Vector3 dir)
    {
        transform.forward = dir;
    }
}
