﻿using Script.System.Ingame;
using System;
using UnityEngine;

public class Enemy_B<DataType> : Character_B<DataType>, IEnemy
    where DataType : CharacterData_B
{
    [SerializeField]
    ScoreData _scoreData;
    public Action<PlayerController> OnAttackEvent;
    protected PlayerController _player;
    protected Camera _camera;
    protected InGameManager _gameManager;
    protected void OnStart()
    {
        _gameManager = ServiceLocator.Get<InGameManager>();
        _camera = Camera.main;
        Start_B();
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

    protected override void Dead()
    {
        base.Dead();
        ServiceLocator.Get<ScoreManager>().AddScore(_scoreData.Score);
        ServiceLocator.Get<EnemyManager>().Remove(this);
        gameObject.SetActive(false);
    }

}
public interface IEnemy : IFightable
{
    void AddOnAttackEvent(Action<PlayerController> action);
    void RemoveOnAttackEvent(Action<PlayerController> action);

    bool IsTargetInView();
}

