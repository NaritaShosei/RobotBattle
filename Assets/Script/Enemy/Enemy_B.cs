using Script.System.Ingame;
using System;
using UnityEngine;

public class Enemy_B<DataType> : Character_B<DataType>, IEnemy
    where DataType : CharacterData_B
{
    public Action<PlayerController> OnAttackEvent;
    protected PlayerController _player;
    Camera _camera;
    protected void OnStart()
    {
        _camera = Camera.main;
    }

    public bool IsTargetInView()
    {
        Vector3 viewportPosition = _camera.WorldToViewportPoint(GetTargetCenter().position);

        // 画面内にいるかチェック
        if (viewportPosition.z < 0) return false;

        if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1) return false;

        return true;
    }
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
}
public interface IEnemy : IFightable
{
    void TargetSet(Collider other);
    void TargetUnset(Collider other);

    void AddOnAttackEvent(Action<PlayerController> action);
    void RemoveOnAttackEvent(Action<PlayerController> action);

    bool IsTargetInView();
}

