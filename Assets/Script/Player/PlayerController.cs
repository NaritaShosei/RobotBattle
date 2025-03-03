﻿using Script.System.Ingame;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Character_B<CharacterData_B>
{
    [SerializeField]
    float _moveSpeed;
    [SerializeField]
    float _jumpSpeed;
    Rigidbody _rb;
    InputBuffer _input;
    Vector2 _velocity;
    bool _isJumped;
    bool _isDashed;
    void Start()
    {
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _rb = GetComponent<Rigidbody>();
        AddAction();
    }

    void Update()
    {
        if (!_isDashed)
        {
            _rb.linearVelocity = new Vector3(_velocity.x, 0, _velocity.y) * _moveSpeed;
        }
        else if (_isDashed)
        {
            _rb.linearVelocity = new Vector3(_velocity.x, 0, _velocity.y) * _moveSpeed * 5;
        }
        if (_isJumped)
        {
            _rb.AddForce(new Vector3(0, _jumpSpeed, 0), ForceMode.Impulse);
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
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isDashed = false;
        }

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
