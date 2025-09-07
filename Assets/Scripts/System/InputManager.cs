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

        //UIとPlayerのActionMapを両方有効化
        _playerMap = _playerInput.actions.FindActionMap("Player", true);
        _uiMap = _playerInput.actions.FindActionMap("UI", true);

        _playerMap.Enable();
        _uiMap.Enable();

        GetAction();

        ServiceLocator.Set(this);
    }
    private void Start()
    {
        //ActionMapの切り替え
        SwitchInputMode(UI);
    }

    /// <summary>
    /// それぞれのアクションを取得
    /// </summary>
    void GetAction()
    {
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

    /// <summary>
    /// ActionMapの切り替え
    /// </summary>
    /// <param name="name">ActionMapの名前</param>
    public void SwitchInputMode(string name)
    {
        if (_playerInput == null) { return; }

        //両方無効化
        _playerMap.Disable();
        _uiMap.Disable();

        //切り替え
        _playerInput.SwitchCurrentActionMap(name);

        if (name == PLAYER)
        {
            _playerMap.Enable();
            //カーソルのロック
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        else if (name == UI)
        {
            _uiMap.Enable();
            //カーソルのロック
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        GetAction();
        Debug.Log($"switch to {name}");
    }
}