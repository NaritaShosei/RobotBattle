﻿using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "GameData/AttackData")]
public class AttackData : ScriptableObject
{
    public float AttackRate => _attackRate;
    [SerializeField]
    float _attackRate = 1f;

    public int BulletCount => _bulletCount;
    [SerializeField]
    int _bulletCount = 30;

    public float ReloadInterval => _reloadInterval;
    [SerializeField]
    float _reloadInterval = 1.5f;

}
