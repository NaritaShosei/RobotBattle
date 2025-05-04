using Script.System.Ingame;
using System;
using UnityEngine;

public class Bullet_B : MonoBehaviour
{
    IFightable _target;
    [SerializeField] BulletData _bulletData;
    [SerializeField] float _moveSpeed = 100;
    [SerializeField] float _minDistance;
    [SerializeField] float _enableTime = 5;
    public Action<Bullet_B> ReturnPoolEvent;
    float _timer;
    bool _isChased = true;
    bool _isTimeReturned;
    bool _isConflictReturned;
    public float GuardBreakValue => _bulletData.GuardBreakValue;
    private void OnEnable()
    {
        _isChased = true;
        _isTimeReturned = false;
        _isConflictReturned = false;
    }
    private void OnDisable()
    {
        if (_isTimeReturned || _isConflictReturned)
        {
            ReturnPoolEvent?.Invoke(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_target != null && (_target.GetTargetCenter().position - transform.position).sqrMagnitude >= _minDistance * _minDistance && _isChased)
        {
            var dir = _target.GetTargetCenter().position - transform.position;
            transform.forward = dir;
        }
        else
        {
            _isChased = false;
        }

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;

        if (_timer >= _enableTime)
        {
            _isTimeReturned = true;
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IgnoreCollider")) return;
        EffectManager.Instance.PlayExplosion(transform.position);
        _isConflictReturned = true;
        gameObject.SetActive(false);
        if (other.TryGetComponent(out IFightable component))
        {
            AddDamage(_bulletData.AttackPower, component);
        }
    }

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
