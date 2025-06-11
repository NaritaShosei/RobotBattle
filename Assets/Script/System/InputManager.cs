using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public const string PLAYER = "Player";
    public const string UI = "UI";

    private PlayerInput _playerInput;

    private InputActionMap _playerMap;
    private InputActionMap _uiMap;
    public InputAction MoveAction { get; private set; }
    public InputAction LookAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public InputAction GuardAction { get; private set; }
    public InputAction AttackAction { get; private set; }
    public InputAction DashAction { get; private set; }
    public InputAction WeaponChangeAction { get; private set; }
    public InputAction ReloadAction { get; private set; }
    public InputAction UINavigateAction { get; private set; }
    public InputAction UISubmitAction { get; private set; }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        //PlayerInputの設定を初期化
        if (_playerInput)
        {
            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        }

        _playerMap = _playerInput.actions.FindActionMap("Player", true);
        _uiMap = _playerInput.actions.FindActionMap("UI", true);

        _playerMap.Enable();
        _uiMap.Enable();

        GetAction();
        ServiceLocator.Set(this);
    }
    private void Start()
    {
        SwitchInputMode(PLAYER);
    }

    void GetAction()
    {
        //それぞれのアクションを取得
        MoveAction = _playerInput.actions["Move"];
        LookAction = _playerInput.actions["Look"];
        JumpAction = _playerInput.actions["Jump"];
        GuardAction = _playerInput.actions["Guard"];
        AttackAction = _playerInput.actions["Attack"];
        DashAction = _playerInput.actions["Dash"];
        WeaponChangeAction = _playerInput.actions["WeaponChange"];
        ReloadAction = _playerInput.actions["Reload"];
        UINavigateAction = _playerInput.actions["Navigate"];
        UISubmitAction = _playerInput.actions["Submit"];
    }


    public void SwitchInputMode(string name)
    {
        if (_playerInput == null) { return; }

        _playerInput.SwitchCurrentActionMap(name);
        GetAction();
    }
}