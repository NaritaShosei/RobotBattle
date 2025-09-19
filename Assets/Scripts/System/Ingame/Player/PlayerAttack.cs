using Cysharp.Threading.Tasks;
using DG.Tweening;
using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] PlayerManager _playerManager;
    [SerializeField] AnimationController _anim;

    private WeaponBase _mainWeapon;
    private WeaponBase _subWeapon;

    InputManager _input;
    WeaponPresenter _presenter;
    AimIK _aimIK;

    float _timer = 0;
    [SerializeField] float _duration = 0.5f;
    [SerializeField] float _ikWeight = 0.846f;
    [SerializeField] private float _swapDuration = 0.5f;

    private IngameManager _gameManager;
    private LockOn _lockOn;
    private PlayerController _playerController;

    [Header("武器を装備する位置")]
    [SerializeField] private Transform _mainParent;
    [SerializeField] private Transform _subParent;

    // 攻撃待機状態
    private bool _waitingForMovement = false;

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

        // PlayerControllerの参照を取得
        _playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
#if UNITY_EDITOR
        //Debug Only
        if (Input.GetKeyDown(KeyCode.Tab))
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

            var weapon = _mainWeapon;
            _mainWeapon = _subWeapon;
            _subWeapon = weapon;

            await SwapWeapon();

            Debug.LogWarning("武装変更");

            //次の武器を装備
            _playerManager.SetState(PlayerState.Idle);

            _mainWeapon.SetAttack(false);
            _mainWeapon.enabled = true;
            _lockOn.SetRange(_mainWeapon.Data.Range);
        }
    }

    private async UniTask SwapWeapon()
    {
        var seq = DOTween.Sequence();

        _mainWeapon.transform.SetParent(_mainParent);
        _subWeapon.transform.SetParent(_subParent);

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
            // 武器がプレイヤーの移動を必要とするかチェック
            if (_mainWeapon.RequiresPlayerMovement())
            {
                // 移動が必要な場合（近接武器など）
                Transform targetTransform = _mainWeapon.GetDesiredPlayerPosition();

                if (targetTransform)
                {
                    // 既に十分近い場合は移動せずに攻撃
                    float distance = Vector3.Distance(_playerController.GetTargetCenter().position, targetTransform.position);
                    if (distance <= _playerController.ArriveThreshold)
                    {
                        StartActualAttack();
                    }
                    else
                    {
                        // PlayerControllerに自動移動を依頼
                        _waitingForMovement = true;
                        _playerController.StartAutoMovement(
                            targetTransform,
                            onComplete: OnAutoMoveComplete,
                            onCanceled: OnAutoMoveCanceled
                        );
                        Debug.Log($"敵に接近を開始: 距離 {distance:F2}m");
                    }
                }
                else
                {
                    StartActualAttack();
                    Debug.LogWarning("攻撃対象が見つかりません");
                    return;
                }
            }
            else
            {
                // 移動が不要な場合（遠距離武器など）
                StartActualAttack();
            }
        }
        //攻撃終了
        else if ((_playerManager.IsState(PlayerState.Attack) ||
                  _playerManager.IsState(PlayerState.MovingToTarget) ||
                  _waitingForMovement) && !isInput)
        {
            // 移動中または攻撃中だった場合
            if (_waitingForMovement || _playerController.IsAutoMoving)
            {
                // 自動移動をキャンセル
                _playerController.CancelAutoMovement();
                _waitingForMovement = false;
                Debug.Log("攻撃をキャンセルしました");
            }
            else
            {
                // 攻撃終了
                EndAttack();
            }
        }
    }

    /// <summary>
    /// 自動移動完了時のコールバック
    /// </summary>
    private void OnAutoMoveComplete()
    {
        _waitingForMovement = false;
        Debug.Log("接近完了、攻撃開始");
        StartActualAttack();
    }

    /// <summary>
    /// 自動移動キャンセル時のコールバック
    /// </summary>
    private void OnAutoMoveCanceled()
    {
        _waitingForMovement = false;
        Debug.Log("接近がキャンセルされました");
        // Idle状態に戻る（PlayerControllerで既に設定済み）
        StartActualAttack();
    }

    /// <summary>
    /// 実際の攻撃開始処理
    /// </summary>
    private void StartActualAttack()
    {
        _playerManager.SetState(PlayerState.Attack);

        _anim.SetBool("IsMissileAttack", true);

        // レイヤー切り替え
        _anim.SetWeight(AnimationLayer.Attack, 1);
        _mainWeapon.IKEnable(_aimIK, true);

        //IKの線形補間の時間初期化
        _timer = 0;

        Debug.Log("攻撃開始");
    }

    /// <summary>
    /// 攻撃終了処理
    /// </summary>
    private void EndAttack()
    {
        _playerManager.SetState(PlayerState.Idle);

        _anim.SetBool("IsMissileAttack", false);

        // レイヤー切り替え
        _anim.SetWeight(AnimationLayer.Attack, 0);

        _mainWeapon.SetAttack(false);
        _mainWeapon.IKEnable(_aimIK, false);

        Debug.Log("攻撃終了");
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
