using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public PlayerState State { get; private set; } = PlayerState.Idle;

    public void SetState(PlayerState newState)
    {
        State = newState;
    }
}
public enum PlayerState
{
    Idle,
    Attack,
    Reload,
    Dash,
    Boost,
    Jump,
    Guard,
    WeaponChange,
    Dead
}