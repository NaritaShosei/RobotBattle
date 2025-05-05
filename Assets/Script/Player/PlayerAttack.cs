using SymphonyFrameWork.System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField]
    PlayerManager _playerManager;

    [SerializeField]
    List<PlayerWeapon> _weapons = new();

    PlayerWeapon _currentWeapon;
    int _index;
    InputBuffer _input;
    bool _isInput;

    void Start()
    {
        _currentWeapon = _weapons[0];
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _input.AttackAction.started += Attack;
        _input.AttackAction.canceled += Attack;
        _input.WeaponChangeAction.started += WeaponChange;
        _input.ReloadAction.started += Reload;
    }

    void Update()
    {

    }
    //TODO:武器変更処理はできてはいるが、直観的ではない部分があるので要・修正
    void WeaponChange(InputAction.CallbackContext context)
    {
        _currentWeapon.IsAttack = false;
        Debug.LogWarning("武装変更");
        _index++;
        _currentWeapon = _weapons[_index % _weapons.Count];
        _currentWeapon.IsAttack = _isInput;
    }

    void Attack(InputAction.CallbackContext context)
    {
        _isInput = context.phase == InputActionPhase.Started;
        _currentWeapon.SetAttack(_isInput);
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
