using Script.System.Ingame;
using UnityEngine;
using UnityEngine.UIElements;

public class BeamBullet : Bullet_B
{
    protected override void Conflict(Collider other)
    {
        EffectManager.Instance.PlayExplosion(transform.position);
        _isConflictReturned = true;
        if (other.TryGetComponent(out IFightable component))
        {
            AddDamage(_bulletData.AttackPower, component);
        }
        gameObject.SetActive(false);
    }

    protected override void OnUpdate()
    {
        _timer += Time.deltaTime;

        transform.position += transform.forward * _bulletData.MoveSpeed * Time.deltaTime;

        if (_timer >= _bulletData.EnableTime)
        {
            _isTimeReturned = true;
            gameObject.SetActive(false);
        }
    }
}
