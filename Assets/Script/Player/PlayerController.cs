using Cysharp.Threading.Tasks;
using DG.Tweening;
using Script.System.Ingame;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    float _currentSpeed;
    bool _isJumped;
    bool _isDashed;
    bool _isBoost;
    Sequence _dashSeq;
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
        Debug.Log(_data.Gauge);
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
            if (!GaugeValueChange(-_data.DashValue * Time.deltaTime))
            {
                _isBoost = false;
                _dashSeq?.Kill();
                _dashSeq = DOTween.Sequence(DOTween.To(() => _currentSpeed, x => _currentSpeed = x, _data.NormalSpeed, 0.8f));
            }
        }
        if (!_isDashed)
        {
            _camForward = Camera.main.transform.forward;
            _camRight = Camera.main.transform.right;
            _camForward.y = _camRight.y = 0;
            _camForward.Normalize();
            _camRight.Normalize();
            _moveDir = _camForward * _velocity.y + _camRight * _velocity.x * 1.5f;

            var vel = _moveDir * _currentSpeed;
            vel.y = _rb.linearVelocity.y;
            _rb.linearVelocity = vel;
        }
        var cam = Camera.main.transform.forward;
        cam.y = 0;
        transform.forward = cam;
    }

    void Move(InputAction.CallbackContext context)
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
            _isDashed = true;
            var vel = _velocity != Vector2.zero ? _moveDir : _camForward;
            _rb.AddForce(new Vector3(vel.x, 0, vel.z) * _data.DashSpeed * 3, ForceMode.Impulse);
            _dashSeq?.Kill();
            _dashSeq = DOTween.Sequence(DOTween.To(() => _currentSpeed, speed => _currentSpeed = speed, _data.DashSpeed, _data.DashTime));
            Dash().Forget();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _dashSeq?.Kill();
            _dashSeq = DOTween.Sequence(DOTween.To(() => _currentSpeed, x => _currentSpeed = x, _data.NormalSpeed, 0.8f));
        }

    }
    async UniTaskVoid Dash()
    {
        await UniTask.Delay((int)(_data.DashTime * 1000));
        _isDashed = false;
        _isBoost = true;
    }


    private void OnDisable()
    {
        RemoveAction();
    }

    void AddAction()
    {
        _input.MoveAction.performed += Move;
        _input.MoveAction.canceled += Move;
        _input.JumpAction.started += Jump;
        _input.JumpAction.canceled += Jump;
        _input.DashAction.started += Dash;
        _input.DashAction.canceled += Dash;
    }

    void RemoveAction()
    {
        _input.MoveAction.performed -= Move;
        _input.MoveAction.canceled -= Move;
        _input.JumpAction.started -= Jump;
        _input.JumpAction.canceled -= Jump;
        _input.DashAction.started -= Dash;
        _input.DashAction.canceled -= Dash;
    }
}
