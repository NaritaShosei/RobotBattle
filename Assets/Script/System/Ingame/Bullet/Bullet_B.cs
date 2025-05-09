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

    public float AttackPower => _bulletData.AttackPower;

    private void OnEnable() => OnEnable_B();

    private void OnDisable() => OnDisable_B();

    protected virtual void OnEnable_B()
    {
        _isTimeReturned = false;
        _isConflictReturned = false;
    }
    protected virtual void OnDisable_B()
    {
        if (_isTimeReturned || _isConflictReturned)
        {
            ReturnPoolEvent?.Invoke(this);
        }
    }
    void Update()
    {
        OnUpdate();
    }
    /// <summary>
    /// Updateで実行したい処理をここに書く
    /// </summary>
    abstract protected void OnUpdate();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IgnoreCollider")) return;
        Conflict(other);
    }
    /// <summary>
    /// ぶつかった時の処理
    /// </summary>
    /// <param name="other"></param>
    protected abstract void Conflict(Collider other);
    public virtual void SetTarget(IFightable target)
    {
        _target = target;
        if (_target != null)
        {
            transform.forward = (target.GetTargetCenter().position - transform.position).normalized;
        }
    }
    public virtual void SetPosition(Vector3 pos)
    {
        _timer = 0;
        transform.position = pos;
    }
}
