using Script.System.Ingame;
using System;
using UnityEngine;

public class Enemy_B : Character_B<EnemyData_B>
{
    public Action<PlayerController> OnAttackEvent;

}
