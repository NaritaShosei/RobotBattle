﻿using Script.System.Ingame;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "GameData/CharacterData/EnemyData")]
public class EnemyData_B : CharacterData_SB
{
    [SerializeField, Header("回避のインターバル")]
    float _dodgeInterval = 5;
    public float DodgeInterval => _dodgeInterval;
    [NonSerialized]
    public float DodgeTimer;

    [SerializeField, Header("上昇時間")]
    float _jumpDuration = 3;
    public float JumpDuration => _jumpDuration;

    [NonSerialized]
    public float JumpTimer;

    [SerializeField, Header("ジャンプのインターバル")]
    float _jumInterval = 5;
    public float JumpInterval => _jumInterval;

    [SerializeField, Header("移動できるPlayerとの最小距離")]
    float _minDistance = 100;
    public float MinDistance => _minDistance;

    [SerializeField, Header("ダッシュに移行する距離")]
    float _dashMinDistance = 200;
    public float DashMinDistance => _dashMinDistance;
    [SerializeField, Header("攻撃に移行する距離")]
    float _attackDistance = 200;
    public float AttackDistance => _attackDistance;
}
