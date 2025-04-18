﻿using Script.System.Ingame;
using System;
using UnityEngine;

public class Bullet_B : MonoBehaviour
{
    Vector3 _moveDirection;
    public Action<Bullet_B> ReturnPoolEvent;
    float _timer;
    float _attackValue;
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
        transform.position += _moveDirection * 100 * Time.deltaTime;
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
    public virtual void SetDirection(Vector3 dir)
    {
        _moveDirection = dir;
    }
    public virtual void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public virtual void SetAttackValue(float attack)
    {
        _attackValue = attack;
    }
}
