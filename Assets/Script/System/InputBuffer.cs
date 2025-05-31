using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(PlayerInput))]
public class InputBuffer : MonoBehaviour
{
    private PlayerInput _playerInput;

    public InputAction MoveAction { get; private set; }
    public InputAction LookAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public InputAction GuardAction { get; private set; }
    public InputAction AttackAction { get; private set; }
    public InputAction DashAction { get; private set; }
    public InputAction WeaponChangeAction { get; private set; }
    public InputAction ReloadAction { get; private set; }


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        //PlayerInputの設定を初期化
        if (_playerInput)
        {
            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        }

        //それぞれのアクションを取得
        MoveAction = _playerInput.actions["Move"];
        LookAction = _playerInput.actions["Look"];
        JumpAction = _playerInput.actions["Jump"];
        GuardAction = _playerInput.actions["Guard"];
        AttackAction = _playerInput.actions["Attack"];
        DashAction = _playerInput.actions["Dash"];
        WeaponChangeAction = _playerInput.actions["WeaponChange"];
        ReloadAction = _playerInput.actions["Reload"];

    }
    private void Start()
    {
        ServiceLocator.Set<InputBuffer>(this);

    }
}
