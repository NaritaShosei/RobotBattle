using Script.System.Ingame;
using System;
using UnityEngine;

public class Enemy_B<DataType> : Character_B<DataType>, IEnemy
    where DataType : CharacterData_B
{
    public Action<PlayerController> OnAttackEvent;
    protected PlayerController _player;

    public void AddOnAttackEvent(Action<PlayerController> action)
    {
        OnAttackEvent += action;
    }

    public void RemoveOnAttackEvent(Action<PlayerController> action)
    {
        OnAttackEvent -= action;
    }

    public virtual void TargetSet(Collider other)
    {

    }

    public virtual void TargetUnset(Collider other)
    {

    }

    protected void OnStart()
    {


    }

    private void Update()
    {

    }

}
public interface IEnemy : IFightable
{
    void TargetSet(Collider other);
    void TargetUnset(Collider other);

    void AddOnAttackEvent(Action<PlayerController> action);
    void RemoveOnAttackEvent(Action<PlayerController> action);
}

