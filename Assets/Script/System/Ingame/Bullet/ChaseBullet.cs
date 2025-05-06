using Script.System.Ingame;
using UnityEngine;

public class ChaseBullet : Bullet_B
{
    bool _isChased = true;

    protected override void OnEnable_B()
    {
        base.OnEnable_B();
        _isChased = true;
    }

    protected override void OnUpdate()
    {
        _timer += Time.deltaTime;
        if (_target != null && (_target.GetTargetCenter().position - transform.position).sqrMagnitude >= _bulletData.MinDistance * _bulletData.MinDistance && _isChased)
        {
            var dir = _target.GetTargetCenter().position - transform.position;
            transform.forward = dir;
        }
        else
        {
            _isChased = false;
        }

        transform.position += transform.forward * _bulletData.MoveSpeed * Time.deltaTime;

        if (_timer >= _bulletData.EnableTime)
        {
            _isTimeReturned = true;
            gameObject.SetActive(false);
        }
    }

    protected override void Conflict(Collider other)
    {
        EffectManager.Instance.PlayExplosion(transform.position);
        _isConflictReturned = true;
        gameObject.SetActive(false);
        if (other.TryGetComponent(out IFightable component))
        {
            AddDamage(_bulletData.AttackPower, component);
        }
    }

}
