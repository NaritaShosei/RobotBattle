using Script.System.Ingame;
using System;
using UnityEngine;

public class Bullet_B : MonoBehaviour
{
    IFightable _target;
    [SerializeField] float _moveSpeed = 100;
    [SerializeField] float _minDistance;
    public Action<Bullet_B> ReturnPoolEvent;
    float _timer;
    float _attackValue;
    bool _isChased = true;
    private void OnEnable()
    {
        _timer = Time.time;
    }
    private void OnDisable()
    {
        ReturnPoolEvent?.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null && (_target.GetTargetCenter().position - transform.position).magnitude >= _minDistance && _isChased)
        {
            var dir = _target.GetTargetCenter().position - transform.position;
            transform.forward = dir;
        }
        else
        {
            _isChased = false;
        }
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        if (_timer + 5 < Time.time)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DodgeCollider")) return;
        EffectManager.Instance.PlayExplosion(transform.position);
        gameObject.SetActive(false);
        Debug.Log(other.name);
        if (other.TryGetComponent(out IFightable component))
        {
            AddDamage(_attackValue, component);
            Debug.Log(other.name);
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
        transform.position = pos;
    }
    public virtual void SetDirection(Vector3 dir)
    {
        transform.forward = dir;
    }
    public virtual void SetAttackValue(float attack)
    {
        _attackValue = attack;
    }
}
