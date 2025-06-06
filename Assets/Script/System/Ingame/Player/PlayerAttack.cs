﻿using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField]
    PlayerManager _playerManager;

    [SerializeField]
    List<PlayerWeapon> _weapons = new();


    [SerializeField]
    AnimationController _anim;

    PlayerWeapon _currentWeapon;
    int _index;
    InputBuffer _input;
    bool _isInput;

    WeaponPresenter _presenter;
    AimIK _aimIK;

    float _timer = 0;
    [SerializeField]
    float _duration = 0.5f;

    void Start()
    {
        _presenter = new WeaponPresenter(ServiceLocator.Get<GameUIManager>().WeaponView);
        _currentWeapon = _weapons[0];
        _weapons[1].enabled = false;
        _input = ServiceLocator.Get<InputBuffer>();
        _input.AttackAction.started += Attack;
        _input.AttackAction.canceled += Attack;
        _input.WeaponChangeAction.started += WeaponChange;
        _input.ReloadAction.started += Reload;
        _aimIK = GetComponent<AimIK>();
        _aimIK.enabled = false;
        _presenter.Initialize(_currentWeapon, _weapons[1]);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("InGame");
        }

        _presenter.CountUpdate(_currentWeapon.Count);

        if (_timer < _duration)
        {
            _timer += Time.deltaTime;
            float t = _timer / _duration;
            float currentWeight = Mathf.Lerp(0f, 0.846f, t);

            _aimIK.solver.IKPositionWeight = currentWeight;
        }
    }
    private void LateUpdate()
    {
        _aimIK.solver.target.position = _currentWeapon.GetTargetPos();
    }
    //TODO:すぐ切り替わってしまうので遅らせる処理が必要
    void WeaponChange(InputAction.CallbackContext context)
    {
        if (_playerManager.IsState(PlayerState.Idle))
        {
            _currentWeapon.IsAttack = false;
            _currentWeapon.enabled = false;
            Debug.LogWarning("武装変更");
            _index++;
            _currentWeapon = _weapons[_index % _weapons.Count];
            _currentWeapon.IsAttack = _isInput;
            _currentWeapon.enabled = true;

            _presenter.SwapWeapon();
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        _isInput = context.phase == InputActionPhase.Started;

        if (_playerManager.IsState(PlayerState.Guard))
        {
            Debug.Log("Guard Now !!");
            return;
        }

        //攻撃開始
        if (_playerManager.IsState(PlayerState.Idle) && _isInput)
        {
            _playerManager.SetState(PlayerState.Attack);

            _anim.SetBool("IsMissileAttack", true);
            // レイヤー切り替え
            _anim.SetWeight(AnimationLayer.Base, 0);
            _anim.SetWeight(AnimationLayer.Attack, 1);
            _currentWeapon.IKEnable(true);
            _timer = 0;
        }

        //攻撃終了
        else if (_playerManager.IsState(PlayerState.Attack) && !_isInput)
        {
            _playerManager.SetState(PlayerState.Idle);

            _anim.SetBool("IsMissileAttack", false);
            _anim.SetWeight(AnimationLayer.Base, 1);
            _anim.SetWeight(AnimationLayer.Attack, 0);
            _currentWeapon.SetAttack(_isInput);
            _currentWeapon.IKEnable(false);
        }
    }

    //AnimationEventで呼び出す、攻撃開始、終了の処理
    void IsAttack()
    {
        _currentWeapon.SetAttack(_isInput);
        Debug.Log($"isInput => {_isInput}");
    }

    //TODO:リロード処理を呼び出す
    void Reload(InputAction.CallbackContext context)
    {
        _currentWeapon.Reload().Forget();
    }

    private void OnDisable()
    {
        _input.AttackAction.started -= Attack;
        _input.AttackAction.canceled -= Attack;
        _input.WeaponChangeAction.started -= WeaponChange;
        _input.ReloadAction.started -= Reload;
    }
}
