using Script.System.Ingame;
using System;
using UnityEngine;

public class EnemyData_B : CharacterData_B
{
    [SerializeField]
    float _dodgeTime = 5;
    public float DodgeTime => _dodgeTime;
    [NonSerialized]
    public float DodgeTimer;

}
