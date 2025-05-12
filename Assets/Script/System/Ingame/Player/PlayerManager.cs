using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public PlayerState State { get; private set; } = PlayerState.Idle;
    /// <summary>
    /// 状態遷移時に呼び出すAction
    /// UIとかに使うかもしれない
    /// </summary>
    Action<PlayerState, PlayerState> OnStateChanged;

    public void SetState(PlayerState newState)
    {
        if (State == newState)
        {
            return;
        }

        bool canTransition = true;

        //攻撃中はガードできない
        if (State == PlayerState.Attack && newState == PlayerState.Guard)
        {
            canTransition = false;
        }

        if (State == PlayerState.Guard && newState == PlayerState.Attack)
        {
            canTransition = false;
        }

        if (canTransition)
        {
            PlayerState oldState = State;

            State = newState;

            OnStateChanged?.Invoke(oldState, newState);
        }
    }

    public bool IsState(PlayerState state)
    {
        return State == state;
    }
}
public enum PlayerState
{
    Idle,
    Attack,
    Reload,
    Guard,
    WeaponChange,
    Dead
}