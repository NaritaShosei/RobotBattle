using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField]
    PlayerManager _playerManager;

    List<WeaponBase> _weapons = new();


    [SerializeField]
    AnimationController _anim;

    WeaponBase _currentWeapon;
    int _index;
    InputManager _input;

    WeaponPresenter _presenter;
    AimIK _aimIK;

    float _timer = 0;
    [SerializeField]
    float _duration = 0.5f;
    [SerializeField]
    float _ikWeight = 0.846f;
    InGameManager _gameManager;

    [SerializeField] private Transform _mainParent;
    [SerializeField] private Transform _subParent;

    void Start()
    {
        _input = ServiceLocator.Get<InputManager>();
        _input.AttackAction.started += Attack;
        _input.AttackAction.canceled += Attack;
        _input.WeaponChangeAction.started += WeaponChange;
        _input.ReloadAction.started += Reload;

        var manager = ServiceLocator.Get<EquipmentManager>();

        _weapons.Add(manager.SpawnWeapon(EquipmentType.Main, _mainParent));
        _weapons.Add(manager.SpawnWeapon(EquipmentType.Sub, _subParent));

        //初期装備の設定
        _currentWeapon = _weapons[0];
        _weapons[1].enabled = false;

        //IKの設定
        _aimIK = GetComponent<AimIK>();
        _aimIK.enabled = false;

        _presenter = new WeaponPresenter(ServiceLocator.Get<GameUIManager>().WeaponView);

        _presenter.Initialize((_currentWeapon.Data.BulletCount, _currentWeapon.Data.WeaponIcon), (_weapons[1].Data.BulletCount, _weapons[1].Data.WeaponIcon));

        _gameManager = ServiceLocator.Get<InGameManager>();
        Debug.LogWarning(_gameManager);
    }

    void Update()
    {
#if UNITY_EDITOR
        //Debug Only
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("InGame");
        }
#endif

        if (_gameManager.IsGameEnd) { _currentWeapon.SetAttack(false); return; }

        if (_gameManager.IsPaused) { return; }

        //残弾数を渡す
        _presenter.CountUpdate(_currentWeapon.Count);

        //腕のIKの線形補間
        if (_timer < _duration)
        {
            _timer += Time.deltaTime;
            float t = _timer / _duration;

            float currentWeight = Mathf.Lerp(0f, _ikWeight, t);

            _aimIK.solver.IKPositionWeight = currentWeight;
        }
    }
    private void LateUpdate()
    {
        if (_gameManager.IsPaused) { return; }

        //IKのtargetの座標を設定する
        _aimIK.solver.target.position = _currentWeapon.GetTargetPos();
    }

    //TODO:すぐ切り替わってしまうので遅らせる処理が必要
    void WeaponChange(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        //Idle状態の時のみ武器変更可能
        if (_playerManager.IsState(PlayerState.Idle))
        {
            //装備中の武器を無効化
            _currentWeapon.SetAttack(false);
            _currentWeapon.enabled = false;

            Debug.LogWarning("武装変更");

            //次の武器を装備

            _currentWeapon = _weapons[_index++ % _weapons.Count];
            _currentWeapon.SetAttack(false);
            _currentWeapon.enabled = true;

            //UIに武器変更の情報を渡す
            _presenter.SwapWeapon();
        }
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        bool isInput = context.phase == InputActionPhase.Started;

        if (_playerManager.IsState(PlayerState.Guard))
        {
            Debug.Log("Guard Now !!");
            return;
        }

        //攻撃開始
        if (_playerManager.IsState(PlayerState.Idle) && isInput)
        {
            _playerManager.SetState(PlayerState.Attack);

            //
            _anim.SetBool("IsMissileAttack", true);

            // レイヤー切り替え
            _anim.SetWeight(AnimationLayer.Attack, 1);
            _currentWeapon.IKEnable(true);

            //IKの線形補間の時間初期化
            _timer = 0;
        }

        //攻撃終了
        else if (_playerManager.IsState(PlayerState.Attack) && !isInput)
        {
            _playerManager.SetState(PlayerState.Idle);

            _anim.SetBool("IsMissileAttack", false);

            // レイヤー切り替え
            _anim.SetWeight(AnimationLayer.Attack, 0);

            _currentWeapon.SetAttack(false);
            _currentWeapon.IKEnable(false);
        }
    }

    //AnimationEventで呼び出す、攻撃開始、終了の処理
    void IsAttack()
    {
        if (_gameManager.IsPaused) { return; }
        _currentWeapon.SetAttack(true);
    }

    void Reload(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        _currentWeapon.Reload();
    }

    private void OnDisable()
    {
        _input.AttackAction.started -= Attack;
        _input.AttackAction.canceled -= Attack;
        _input.WeaponChangeAction.started -= WeaponChange;
        _input.ReloadAction.started -= Reload;
    }
}
