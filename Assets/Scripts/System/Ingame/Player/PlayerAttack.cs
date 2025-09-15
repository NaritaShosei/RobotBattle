using Cysharp.Threading.Tasks;
using DG.Tweening;
using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField]
    PlayerManager _playerManager;

    [SerializeField]
    AnimationController _anim;

    private WeaponBase _mainWeapon;
    private WeaponBase _subWeapon;

    InputManager _input;

    WeaponPresenter _presenter;
    AimIK _aimIK;

    float _timer = 0;
    [SerializeField]
    float _duration = 0.5f;
    [SerializeField]
    float _ikWeight = 0.846f;
    [SerializeField]
    private float _swapDuration = 0.5f;

    private IngameManager _gameManager;
    private LockOn _lockOn;

    [Header("武器を装備する位置")]
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

        //初期装備の設定
        _mainWeapon = manager.SpawnWeapon(EquipmentType.Main, _mainParent);
        _subWeapon = manager.SpawnWeapon(EquipmentType.Sub, _subParent);

        //IKの設定
        _aimIK = GetComponent<AimIK>();
        _aimIK.enabled = false;

        _presenter = new WeaponPresenter(ServiceLocator.Get<GameUIManager>().WeaponView);

        _presenter.Initialize((_mainWeapon.Data.AttackCapacity, _mainWeapon.Data.WeaponIcon), (_subWeapon.Data.AttackCapacity, _subWeapon.Data.WeaponIcon));

        _gameManager = ServiceLocator.Get<IngameManager>();

        _lockOn = ServiceLocator.Get<LockOn>();
        _lockOn.SetRange(_mainWeapon.Data.Range);
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

        if (_gameManager.IsGameEnd) { _mainWeapon.SetAttack(false); return; }

        if (_gameManager.IsPaused) { return; }

        //残弾数を渡す
        _presenter.CountUpdate(_mainWeapon.Count);

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
        _aimIK.solver.target.position = _mainWeapon.GetTargetPos();
    }

    async void WeaponChange(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        //Idle状態の時のみ武器変更可能
        if (_playerManager.IsState(PlayerState.Idle))
        {
            _playerManager.SetState(PlayerState.WeaponChange);

            //装備中の武器を無効化
            _mainWeapon.SetAttack(false);
            _mainWeapon.enabled = false;

            //UIに武器変更の情報を渡す
            _presenter.SwapWeapon();

            await SwapWeapon();

            Debug.LogWarning("武装変更");

            //次の武器を装備
            _playerManager.SetState(PlayerState.Idle);

            var weapon = _mainWeapon;
            _mainWeapon = _subWeapon;
            _subWeapon = weapon;

            _mainWeapon.SetAttack(false);
            _mainWeapon.enabled = true;
            _lockOn.SetRange(_mainWeapon.Data.Range);
        }
    }

    private async UniTask SwapWeapon()
    {
        var seq = DOTween.Sequence();

        _mainWeapon.transform.SetParent(_subParent);
        _subWeapon.transform.SetParent(_mainParent);

        await seq.Append(_mainWeapon.transform.DOLocalMove(Vector3.zero, _swapDuration)).
            Join(_subWeapon.transform.DOLocalMove(Vector3.zero, _swapDuration)).
            AsyncWaitForCompletion();
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
            _mainWeapon.IKEnable(_aimIK, true);

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

            _mainWeapon.SetAttack(false);
            _mainWeapon.IKEnable(_aimIK, false);
        }
    }

    //AnimationEventで呼び出す、攻撃開始、終了の処理
    void IsAttack()
    {
        if (_gameManager.IsPaused) { return; }
        _mainWeapon.SetAttack(true);
    }

    void Reload(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        _mainWeapon.Reload();
    }

    private void OnDisable()
    {
        _input.AttackAction.started -= Attack;
        _input.AttackAction.canceled -= Attack;
        _input.WeaponChangeAction.started -= WeaponChange;
        _input.ReloadAction.started -= Reload;
    }
}
