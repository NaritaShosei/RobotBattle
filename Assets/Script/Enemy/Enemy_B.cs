using Script.System.Ingame;
using System;
using UnityEngine;

public class Enemy_B : Character_B<EnemyData_B>
{
    [SerializeField] protected EnemyData_B _dataBase;
    public Action<PlayerController> OnAttackEvent;
    protected PlayerController _player;

    protected void OnStart()
    {
        
        
    }

    private void Update()
    {
        
    }
}
