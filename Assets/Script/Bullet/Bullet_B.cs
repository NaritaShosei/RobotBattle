using Script.System.Ingame;
using System;
using UnityEngine;

public class Bullet_B : MonoBehaviour
{
    Vector3 _moveDirection;
    public Action<Bullet_B> ReturnPoolEvent;
    float _timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _moveDirection * 100 * Time.deltaTime;
        if (_timer + 10 < Time.time)
        {
            gameObject.SetActive(false);
            ReturnPoolEvent?.Invoke(this);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        ReturnPoolEvent?.Invoke(this);
        if (other.TryGetComponent(out IFightable component))
        {
            AddDamage(0, component);
        }
    }

    protected virtual void AddDamage(float damage, IFightable fightable)
    {

    }
    public virtual void SetDirection(Vector3 dir)
    {
        _moveDirection = dir;
        _timer = Time.time;
    }
    public virtual void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
