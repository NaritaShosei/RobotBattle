using Script.System.Ingame;
using UnityEngine;

public class ChaseBullet : Bullet_B
{
    bool _isChased = true;
    [SerializeField] private float _minDistance = 10;

    protected override void OnEnable_B()
    {
        base.OnEnable_B();
        _isChased = true;
    }

    protected override void OnUpdate()
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

        transform.position += transform.forward * _weaponData.AttackSpeed * Time.deltaTime;

        if (_timer >= _enableTime)
        {
            _isTimeReturned = true;
            gameObject.SetActive(false);
        }
    }

    protected override void Conflict(Collider other)
    {
        ServiceLocator.Get<EffectManager>().PlayExplosion(transform.position);
        _isConflictReturned = true;
        gameObject.SetActive(false);
    }

}
