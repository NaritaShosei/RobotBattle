using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Script.System.Ingame;
using SymphonyFrameWork.System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Character_B<CharacterData_B>
{
    [SerializeField]
    float _normalSpeed;
    [SerializeField]
    float _dashSpeed;
    [SerializeField]
    float _jumpSpeed;
    Rigidbody _rb;
    InputBuffer _input;
    Vector2 _velocity;
    float _currentSpeed;
    bool _isJumped;
    bool _isDashed;
    TweenerCore<float, float, FloatOptions> _dashTween = null;
    void Start()
    {
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _rb = GetComponent<Rigidbody>();
        _currentSpeed = _normalSpeed;
        AddAction();
    }

    void Update()
    {
        if (_isJumped)
        {
            _rb.AddForce(new Vector3(0, _jumpSpeed, 0), ForceMode.Impulse);
        }
        if (!_isDashed)
        {
            _rb.linearVelocity = new Vector3(_velocity.x, 0, _velocity.y) * _currentSpeed;
        }
        else if (_isDashed)
        {

        }
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
            _isJumped = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isJumped = false;
        }
    }
    void Dash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _isDashed = true;
            var vel = _velocity != Vector2.zero ? _velocity : new Vector2(0, 1);
            _rb.AddForce(new Vector3(vel.x, 0, vel.y) * _dashSpeed * 3, ForceMode.Impulse);
            _dashTween?.Kill();
            _dashTween = DOTween.To(() => _currentSpeed, x => _currentSpeed = x, _dashSpeed, 0.5f);
            StartCoroutine(Dash());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _dashTween?.Kill();
            _dashTween = DOTween.To(() => _currentSpeed, x => _currentSpeed = x, _normalSpeed, 0.8f);
        }

    }

    IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.5f);
        _isDashed = false;
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
