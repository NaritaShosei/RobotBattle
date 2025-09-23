using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public const string PLAYER = "Player";
    public const string UI = "UI";

    private InputSystem_Actions _inputActions;

    [SerializeField] private PlayerInput _playerInput;

    public InputAction MoveAction { get; private set; }
    public InputAction LookAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public InputAction GuardAction { get; private set; }
    public InputAction AttackAction { get; private set; }
    public InputAction DashAction { get; private set; }
    public InputAction WeaponChangeAction { get; private set; }
    public InputAction ReloadAction { get; private set; }
    public InputAction SpecialAction { get; private set; }
    public InputAction UINavigateAction { get; private set; }
    public InputAction UISubmitAction { get; private set; }

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();

        if (!_playerInput)
            _playerInput = GetComponent<PlayerInput>();

        // 生成したアクションをPlayerInputにセット
        _playerInput.actions = _inputActions.asset;

        _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

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
        MoveAction = _inputActions.Player.Move;
        LookAction = _inputActions.Player.Look;
        JumpAction = _inputActions.Player.Jump;
        GuardAction = _inputActions.Player.Guard;
        AttackAction = _inputActions.Player.Attack;
        DashAction = _inputActions.Player.Dash;
        WeaponChangeAction = _inputActions.Player.WeaponChange;
        ReloadAction = _inputActions.Player.Reload;
        SpecialAction = _inputActions.Player.Special;

        UINavigateAction = _inputActions.UI.Navigate;
        UISubmitAction = _inputActions.UI.Submit;
    }

    /// <summary>
    /// ActionMapの切り替え
    /// </summary>
    /// <param name="name">ActionMapの名前</param>
    public void SwitchInputMode(string name)
    {
        if (_playerInput == null) { return; }


        //切り替え
        _playerInput.SwitchCurrentActionMap(name);

        if (name == PLAYER)
        {
            //カーソルのロック
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        else if (name == UI)
        {
            //カーソルのロック
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Debug.Log($"switch to {name}");
    }
}