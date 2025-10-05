using Script.System.Ingame;
using UnityEngine;
using UnityEngine.UIElements;

public class BeamBullet : Bullet_B
{
    protected override void OnUpdate()
    {
        _timer += Time.deltaTime;

        transform.position += transform.forward * _weaponData.AttackSpeed * Time.deltaTime;

        if (_timer >= _enableTime)
        {
            _isTimeReturned = true;
            gameObject.SetActive(false);
        }
    }
}
