using Cysharp.Threading.Tasks;
using DG.Tweening;
using Script.System.Ingame;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Character_B<CharacterData_B>
{
    [SerializeField]
    CharacterData_B _dataBase;

    Rigidbody _rb;
    InputBuffer _input;
    Vector2 _velocity;
    Vector3 _camForward;
    Vector3 _camRight;
    Vector3 _moveDir;
    Vector3 _dashStartPos;
    Vector3 _dashTargetPos;
    float _currentSpeed;
    bool _isJumped;
    bool _isDashed;
    bool _isBoost;
    /// <summary>
    /// Debug用
    /// </summary>
    [SerializeField] Text a;
    void Start()
    {
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _rb = GetComponent<Rigidbody>();
        Initialize(_dataBase);
        _currentSpeed = _data.NormalSpeed;
        AddAction();
    }

    void Update()
    {
        //Debug用
        a.text = _data.Gauge.ToString();
        if (!_isDashed && !_isJumped)
        {
            GaugeValueChange(_data.RecoveryValue * Time.deltaTime);
        }
        if (_isJumped)
        {
            if (!GaugeValueChange(-_data.JumpValue * Time.deltaTime))
            {
                _isJumped = false;
            }
            else
            {
                _rb.AddForce(new Vector3(0, _data.JumpPower, 0), ForceMode.Impulse);
            }
        }
        if (_isBoost)
        {
            if (_velocity != Vector2.zero)
            {
                if (!GaugeValueChange(-_data.DashValue * Time.deltaTime))
                {
                    _isBoost = false;
                }
            }
        }

        if (_isDashed)
        {
            _data.DashTimer += Time.deltaTime;

            var t = Mathf.Clamp01(_data.DashTimer / _data.DashTime);

            var newPos = Vector3.Lerp(_dashStartPos, _dashTargetPos, t);
            _rb.MovePosition(newPos);

            if (t >= 1)
            {
                _isDashed = false;
            }
        }

        if (!_isDashed)
        {
            Move(_isBoost ? _data.BoostSpeed : _data.NormalSpeed);
        }
        var cam = Camera.main.transform.forward;
        cam.y = 0;
        transform.forward = cam;
    }

    void OnMoveInput(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Performed)
        {
            _velocity = input;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _velocity = Vector2.zero;
        }
        if (_velocity.magnitude > 1)
        {
            _velocity = _velocity.normalized;
        }
    }

    void Move(float speed)
    {
        _camForward = Camera.main.transform.forward;
        _camRight = Camera.main.transform.right;
        _camForward.y = _camRight.y = 0;
        _camForward.Normalize();
        _camRight.Normalize();
        _moveDir = _camForward * _velocity.y + _camRight * _velocity.x * 1.5f;

        var vel = _moveDir * speed;
        var currentVel = _rb.linearVelocity;
        vel.y = currentVel.y;
        _rb.linearVelocity = vel;
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!GaugeValueChange(-_data.JumpValue / 10)) return;
            _isJumped = true;
            _rb.AddForce(new Vector3(0, _data.JumpPower * 5, 0), ForceMode.Impulse);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isJumped = false;
        }
    }

    //HERE:Dashの仕様を線形補完に変更する
    void Dash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !_isDashed)
        {
            if (!GaugeValueChange(-_data.DashValue)) return;
            _isBoost = true;
            _isDashed = true;
            _data.DashTimer = 0;
            _dashStartPos = transform.position;
            _dashTargetPos = transform.position + (_velocity != Vector2.zero ? _moveDir : _camForward) * _data.DashDistance;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            _isBoost = false;
        }
    }
    private void OnDisable()
    {
        RemoveAction();
    }

    void AddAction()
    {
        _input.MoveAction.performed += OnMoveInput;
        _input.MoveAction.canceled += OnMoveInput;
        _input.JumpAction.started += Jump;
        _input.JumpAction.canceled += Jump;
        _input.DashAction.started += Dash;
        _input.DashAction.canceled += Dash;
    }

    void RemoveAction()
    {
        _input.MoveAction.performed -= OnMoveInput;
        _input.MoveAction.canceled -= OnMoveInput;
        _input.JumpAction.started -= Jump;
        _input.JumpAction.canceled -= Jump;
        _input.DashAction.started -= Dash;
        _input.DashAction.canceled -= Dash;
    }
}
