using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(PlayerInput))]
public class InputBuffer : MonoBehaviour
{
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
    public InputAction UISelectAction { get; private set; }

    Action _playerAction;

    ModeType _currentMode;
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

        GetPlayerAction();
        ServiceLocator.Set(this);
    }
    private void Start()
    {
        SwitchInputMode(ModeType.UI);
    }

    void GetPlayerAction()
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
        UISelectAction = _playerInput.actions["Navigate"];
    }

    public void AddAction(Action action)
    {
        _playerAction += action;
    }


    public void SwitchInputMode(ModeType mode)
    {
        if (_playerInput == null) { return; }
        if (_currentMode == mode) { return; }

        switch (mode)
        {
            case ModeType.Player:
                _playerInput.SwitchCurrentActionMap("Player");
                _playerAction?.Invoke();
                GetPlayerAction();
                break;
            case ModeType.UI:
                _playerInput.SwitchCurrentActionMap("UI");
                Debug.LogWarning("UI");
                break;
        }

        _currentMode = mode;
    }


}
public enum ModeType
{
    Player,
    UI,
}