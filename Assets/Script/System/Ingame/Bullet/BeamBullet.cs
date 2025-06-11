using Script.System.Ingame;
using UnityEngine;
using UnityEngine.UIElements;

public class BeamBullet : Bullet_B
{
    protected override void Conflict(Collider other)
    {
        ServiceLocator.Get<EffectManager>().PlayExplosion(transform.position);
        _isConflictReturned = true;
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
