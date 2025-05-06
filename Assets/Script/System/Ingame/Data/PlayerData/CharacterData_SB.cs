using Script.System.Ingame;
using System;
using UnityEngine;


public class CharacterData_SB : CharacterData_B
{
    [SerializeField]
    private float _normalSpeed = 50;
    public float NormalSpeed => _normalSpeed;
    [SerializeField]
    private float _boostSpeed = 150;
    public float BoostSpeed => _boostSpeed;
    [SerializeField]
    private float _jumpPower = 30f;
    public float JumpPower => _jumpPower;
    [SerializeField]
    private float _floatPower = 200f;
    public float FloatPower => _floatPower;

    [SerializeField]
    private float _dashDistance = 100;
    public float DashDistance => _dashDistance;

    [SerializeField]
    private float _dashTime = 0.5f;
    public float DashTime => _dashTime;

    [NonSerialized]
    public float DashTimer;

    [SerializeField, Header("自由落下速度")]
    private float _fallSpeed = 20f;
    public float FallSpeed => _fallSpeed;

    [SerializeField, Header("1秒間に回復するゲージ量")]
    private float _recoveryValue = 30;
    public float RecoveryValue => _recoveryValue;

    [SerializeField, Header("ジャンプの消費ゲージ量")]
    private float _jumpValue = 50;
    public float JumpValue => _jumpValue;

    [SerializeField, Header("ダッシュの消費ゲージ量")]
    private float _dashValue = 50;
    public float DashValue => _dashValue;
}
