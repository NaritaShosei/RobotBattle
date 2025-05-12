using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public PlayerState State { get; private set; } = PlayerState.Idle;

    Action<PlayerState, PlayerState> OnStateChanged;

    public void SetState(PlayerState newState)
    {
        if (State == newState)
        {
            Debug.Log("同じ状態に遷移しようとしています");
            return;
        }

        bool canTransition = true;

        //攻撃中はガードできない
        if (State == PlayerState.Attack && newState == PlayerState.Guard)
        {
            Debug.Log("攻撃");
            canTransition = false;
        }

        if (State == PlayerState.Guard && newState == PlayerState.Attack)
        {
            Debug.Log("ガード");
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
        Debug.Log($"State => {State} : state=> {state}");
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