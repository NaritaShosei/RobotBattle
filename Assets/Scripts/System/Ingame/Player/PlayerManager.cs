using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private AnimationController _animController;

    public AnimationController AnimController => _animController;
    public PlayerState State { get; private set; } = PlayerState.Idle;

    public event Action OnDead;

    /// <summary>
    /// 状態遷移時に呼び出すAction
    /// UIとかに使うかもしれない
    /// </summary>
    public Action<PlayerState, PlayerState> OnStateChanged;

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

        if (State == PlayerState.Dead) { OnDead?.Invoke(); }
    }

    public bool IsState(PlayerState state)
    {
        return State == state;
    }
}
public enum PlayerState
{
    Idle,           // 通常状態
    Attack,         // 攻撃
    Reload,　　   　// リロード
    Guard,          // ガード
    WeaponChange,   // 武器交換
    Dead,           // 死亡
    MovingToTarget, // 自動移動
    AttackReady,    // 攻撃待機
    SpecialAttack   // 必殺技
}