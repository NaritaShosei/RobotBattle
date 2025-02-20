using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer : MonoBehaviour
{
    private PlayerInput _playerInput;
    
    public InputAction MoveAction { get; private set; }
    public InputAction LookAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public InputAction GuardAction { get; private set; }
    public InputAction Attack1Action {get; private set;}
    public InputAction Attack2Action {get; private set;}
    public InputAction Attack3Action {get; private set;}
    

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
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
        Attack1Action = _playerInput.actions["Attack_1"];
        Attack2Action = _playerInput.actions["Attack_2"];
        Attack3Action = _playerInput.actions["Attack_3"];
    }

}
