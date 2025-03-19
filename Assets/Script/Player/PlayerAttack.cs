using SymphonyFrameWork.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    AttackData _data;
    [SerializeField]
    GameObject _attack1Bullet;
    Queue<GameObject> _attack1BulletPool = new();
    InputBuffer _input;
    bool _isAttacked1;
    void Start()
    {
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _input.Attack1Action.started += Attack1;
        _input.Attack1Action.canceled += Attack1;
        for (int i = 0; i < 30; i++)
        {
            var bullet = Instantiate(_attack1Bullet);
            bullet.SetActive(false);
            _attack1BulletPool.Enqueue(bullet);
        }
    }

    void Update()
    {
        if (_isAttacked1)
        {
            float rate = 1 / _data.AttackRate;

        }
    }

    void Attack1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _isAttacked1 = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isAttacked1 = false;
        }
    }
    private void OnDisable()
    {
        _input.Attack1Action.started -= Attack1;
        _input.Attack1Action.canceled -= Attack1;
    }
}
