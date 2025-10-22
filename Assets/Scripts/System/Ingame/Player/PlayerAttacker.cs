using Cysharp.Threading.Tasks;
using DG.Tweening;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private float _animationWeight = 0.5f;

    private Weapon_B _mainWeapon;
    private Weapon_B _subWeapon;
    private SpecialAttack_B _specialAttack;

    private InputManager _input;
    private WeaponPresenter _presenter;
    private AimIK _aimIK;
    private Vector3 _ikAxis;

    private float _timer = 0;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.3f; // フェードアウト専用時間
    [SerializeField] private float _ikWeight = 0.846f;
    [SerializeField] private float _swapDuration = 0.5f;

    private IngameManager _gameManager;
    private LockOn _lockOn;
    private PlayerController _playerController;
    private PlayerEquipmentManager _playerEquipmentManager;

    // 攻撃待機状態
    private bool _waitingForMovement = false;
    // 攻撃中の回転継続フラグ
    private bool _isRotatingDuringAttack = false;

    // IK制御用の新しいフラグ
    private bool _isIKActive = false;
    private bool _shouldResetIK = false;
    private bool _isIKFadingOut = false; // フェードアウト中フラグ
    private float _currentIKWeight = 0f; // 現在のIKウェイト値を記録

    private SpecialGaugeModel _specialGauge;

    private void Start()
    {
        InitializeReferences();
        SetupWeapons();
        SetupSpecial();
        SetupUI();
        SetupIK();
        SetupInput();
    }

    #region 初期化処理
    private void InitializeReferences()
    {
        _input = ServiceLocator.Get<InputManager>();
        _gameManager = ServiceLocator.Get<IngameManager>();
        _lockOn = ServiceLocator.Get<LockOn>();
        _playerController = GetComponent<PlayerController>();
        _playerEquipmentManager = ServiceLocator.Get<PlayerEquipmentManager>();
    }

    private void SetupWeapons()
    {
        var manager = ServiceLocator.Get<EquipmentManager>();

        //初期装備の設定
        _mainWeapon = manager.SpawnWeapon(_playerEquipmentManager, WeaponType.Main);
        _subWeapon = manager.SpawnWeapon(_playerEquipmentManager, WeaponType.Sub);

        var mainParent = _playerEquipmentManager.GetEquipmentParent(_mainWeapon.Data.EquipmentType);
        var subParent = _playerEquipmentManager.GetEquipmentParent(_subWeapon.Data.EquipmentType);

        _mainWeapon.transform.rotation = mainParent.transform.rotation;
        _subWeapon.transform.rotation = subParent.transform.rotation;

        _mainWeapon.PlayDissolveEffect(true, 0);

        _subWeapon.gameObject.SetActive(false);
    }

    private void SetupSpecial()
    {
        _specialGauge = new SpecialGaugeModel();

        new SpecialGaugePresenter(_specialGauge, ServiceLocator.Get<GameUIManager>().SpecialGaugeView);

        var manager = ServiceLocator.Get<EquipmentManager>();

        _specialAttack = manager.SpawnSpecial(_playerEquipmentManager);

        _specialGauge.Initialize(_specialAttack.Data.RequiredGauge);
    }

    private void SetupInput()
    {
        _input.AttackAction.started += Attack;
        _input.AttackAction.canceled += Attack;
        _input.WeaponChangeAction.started += WeaponChange;
        _input.ReloadAction.started += Reload;
        _input.SpecialAction.started += Special;
    }

    private void SetupIK()
    {
        //IKの設定
        _aimIK = GetComponent<AimIK>();
        _ikAxis = _aimIK.solver.axis;
        _aimIK.enabled = false;
        _aimIK.solver.IKPositionWeight = 0f;
    }

    private void SetupUI()
    {
        _presenter = new WeaponPresenter(ServiceLocator.Get<GameUIManager>().WeaponView);
        _presenter.Initialize((_mainWeapon.Data.AttackCapacity, _mainWeapon.Data.WeaponIcon), (_subWeapon.Data.AttackCapacity, _subWeapon.Data.WeaponIcon));

        _lockOn.SetRange(_mainWeapon.Data.Range);
    }

    #endregion

    private void Update()
    {
        if (_gameManager.IsGameEnd)
        {
            _mainWeapon.SetAttack(false);
            // 攻撃終了時に回転も停止
            if (_isRotatingDuringAttack)
            {
                _playerController.StopTargetRotation();
                _isRotatingDuringAttack = false;
            }
            // IKも強制リセット
            ForceResetIK();
            return;
        }

        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        // Debug用に適当なロジック
        _specialGauge.UpdateValue(Time.deltaTime);

        //残弾数を渡す
        // TODO:イベント駆動にしたほうがきれい
        _presenter.CountUpdate(_mainWeapon.Count);

        // IK制御の改善
        UpdateIK();
    }

    /// <summary>
    /// IK制御
    /// </summary>
    private void UpdateIK()
    {
        if (_aimIK == null) return;

        // フェードアウト中の処理
        if (_isIKFadingOut)
        {
            _timer += Time.deltaTime;
            float t = Mathf.Clamp01(_timer / _fadeOutDuration);

            _currentIKWeight = Mathf.Lerp(_currentIKWeight, 0f, t);
            _aimIK.solver.IKPositionWeight = _currentIKWeight;

            if (t >= 1f || _currentIKWeight < 0.001f)
            {
                // フェードアウト完了
                _aimIK.solver.IKPositionWeight = 0f;
                _aimIK.enabled = false;
                _isIKActive = false;
                _isIKFadingOut = false;
                _shouldResetIK = false;
                _currentIKWeight = 0f;
                _timer = 0f;
                Debug.Log("IKフェードアウト完了");
            }
            return;
        }

        // 攻撃状態以外では滑らかにフェードアウト
        if (!_playerManager.IsState(PlayerState.Attack))
        {
            if (_aimIK.enabled && _currentIKWeight > 0f && !_isIKFadingOut)
            {
                StartIKFadeOut();
                return;
            }
            else if (!_aimIK.enabled && _currentIKWeight <= 0f)
            {
                // 既に無効化済みの場合はそのまま
                return;
            }
        }

        // 強制リセットが必要な場合
        if (_shouldResetIK)
        {
            _aimIK.solver.IKPositionWeight = 0f;
            _aimIK.enabled = false;
            _isIKActive = false;
            _shouldResetIK = false;
            _isIKFadingOut = false;
            _currentIKWeight = 0f;
            _timer = 0f;
            Debug.Log("IKを強制リセットしました");
            return;
        }

        // IKが有効でフェードイン中の場合
        if (_isIKActive && _aimIK.enabled && _timer < _duration && !_isIKFadingOut)
        {
            _timer += Time.deltaTime;
            float t = Mathf.Clamp01(_timer / _duration);

            _currentIKWeight = Mathf.Lerp(0f, _ikWeight, t);
            _aimIK.solver.IKPositionWeight = _currentIKWeight;
        }
        // IKが有効でなく、現在ウェイトが残っている場合は滑らかにフェードアウト
        else if (!_isIKActive && _currentIKWeight > 0f && !_isIKFadingOut)
        {
            StartIKFadeOut();
        }
    }

    private void LateUpdate()
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

        //IKのtargetの座標を設定する
        if (_aimIK != null && _aimIK.enabled && _isIKActive)
        {
            _aimIK.solver.target.position = _mainWeapon.GetTargetPos();
        }
    }

    async void WeaponChange(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        //Idle状態の時のみ武器変更可能
        if (_playerManager.IsState(PlayerState.Idle))
        {
            _playerManager.SetState(PlayerState.WeaponChange);

            // 回転制御を停止
            if (_isRotatingDuringAttack)
            {
                _playerController.StopTargetRotation();
                _isRotatingDuringAttack = false;
            }

            // IKを強制リセット
            ForceResetIK();

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
        _mainWeapon.gameObject.SetActive(true);

        _mainWeapon.PlayDissolveEffect(true, _swapDuration);
        _subWeapon.PlayDissolveEffect(false, _swapDuration);

        // 装備位置への移動
        await UniTask.Delay((int)(1000 * _swapDuration));
        _subWeapon.gameObject.SetActive(false);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        bool isInput = context.phase == InputActionPhase.Started;

        //攻撃開始
        if (_playerManager.IsState(PlayerState.Idle) && isInput)
        {
            // 武器がプレイヤーの移動を必要とするかチェック
            if (_mainWeapon.RequiresPlayerMovement())
            {
                // 移動が必要な場合（近接武器など）
                Transform targetTransform = _mainWeapon.GetTarget();

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
                            _mainWeapon.Data.AttackSpeed,
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
        // IKもリセット
        ForceResetIK();
        Debug.Log("接近がキャンセルされました");
        // Idle状態に戻る（PlayerControllerで既に設定済み）
    }

    /// <summary>
    /// 実際の攻撃開始処理
    /// </summary>
    private void StartActualAttack()
    {
        _playerManager.SetState(PlayerState.Attack);

        var animData = _mainWeapon.Data.WeaponAnimationData;

        // アニメーション再生可能な武器、状況であればアニメーション再生
        if (_mainWeapon.CanAttackAnimPlay())
        {
            _playerManager.AnimController.SetAttack(animData.AttackTrigger, animData.AnimationType);

            // レイヤー切り替え
            _playerManager.AnimController.SetWeight(animData.AnimationLayer, animData.AttackLayerWeight);

            // IKを有効化
            EnableIK();
        }

        // 近接武器の場合、攻撃中も目標への回転を継続
        if (_mainWeapon.RequiresPlayerMovement())
        {
            Transform target = _mainWeapon.GetTarget();
            if (target != null)
            {
                _playerController.StartTargetRotation();
                _isRotatingDuringAttack = true;
                Debug.Log("攻撃中の目標回転を開始");
            }
        }

        Debug.Log("攻撃開始");
    }

    /// <summary>
    /// 攻撃終了処理
    /// </summary>
    private void EndAttack()
    {
        _playerManager.SetState(PlayerState.Idle);

        // IKを自然にフェードアウト
        StartIKFadeOut();

        var animData = _mainWeapon.Data.WeaponAnimationData;

        _playerManager.AnimController.ResetAttack(animData.AttackTrigger, animData.AnimationType);

        // レイヤー切り替え
        _playerManager.AnimController.SetWeight(animData.AnimationLayer, 0);

        _mainWeapon.SetAttack(false);

        // 攻撃中の回転を停止
        if (_isRotatingDuringAttack)
        {
            _playerController.StopTargetRotation();
            _isRotatingDuringAttack = false;
            Debug.Log("攻撃中の目標回転を停止");
        }

        Debug.Log("攻撃終了");
    }

    /// <summary>
    /// IKを有効化する
    /// </summary>
    private void EnableIK()
    {
        if (_aimIK != null && _mainWeapon.IKEnable())
        {
            _aimIK.enabled = true;
            _aimIK.solver.axis = _ikAxis;
            _isIKActive = true;
            _shouldResetIK = false;
            _isIKFadingOut = false;
            _timer = 0f; // タイマーをリセット
            // 現在のウェイトから開始（滑らかな継続）
            _currentIKWeight = _aimIK.solver.IKPositionWeight;
            Debug.Log("IKを有効化しました");
        }
    }

    /// <summary>
    /// IKのフェードアウトを開始
    /// </summary>
    private void StartIKFadeOut()
    {
        if (_aimIK != null && (_aimIK.enabled || _currentIKWeight > 0f))
        {
            _isIKActive = false;
            _isIKFadingOut = true;
            _shouldResetIK = false;
            _timer = 0f; // フェードアウト用にタイマーリセット
            // 現在のウェイト値を保持してそこからフェードアウト
            _currentIKWeight = _aimIK.solver.IKPositionWeight;
        }
        else
        {
            // 既に無効な場合は即座にリセット
            ForceResetIK();
        }
    }

    /// <summary>
    /// IKをリセットする
    /// </summary>
    private void ResetIK()
    {
        _shouldResetIK = true;
        Debug.Log("IKリセットをスケジュール");
    }

    /// <summary>
    /// IKを強制的にリセットする（即座に実行）
    /// </summary>
    private void ForceResetIK()
    {
        if (_aimIK != null)
        {
            _aimIK.solver.IKPositionWeight = 0f;
            _aimIK.enabled = false;
            _isIKActive = false;
            _shouldResetIK = false;
            _isIKFadingOut = false;
            _currentIKWeight = 0f;
            _timer = 0f;
            Debug.Log("IKを強制リセットしました");
        }
    }

    //AnimationEventで呼び出す、攻撃開始、終了の処理
    private void IsAttack()
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }
        _mainWeapon.SetAttack(true);
    }

    private void Reload(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        _mainWeapon.Reload();

        // リロードのアニメーション再生
        var animData = _mainWeapon.Data.WeaponAnimationData;

        _playerManager.AnimController.SetTrigger(animData.ReloadTrigger);
    }

    private async void Special(InputAction.CallbackContext context)
    {
        // 必殺技中の重複実行を防止
        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        if (_playerManager.IsState(PlayerState.Idle) && _specialGauge.TryUseGauge())
        {
            // 必殺技状態に変更（他の処理より先に）
            _playerManager.SetState(PlayerState.SpecialAttack);

            // 必殺技開始前にすべての動作を停止
            StopAllMovementAndRotation();

            try
            {
                await _specialAttack.SpecialAttack();
            }
            catch (OperationCanceledException ex) { }
            catch (Exception ex) { Debug.LogWarning(ex.Message); }

            _playerManager.SetState(PlayerState.Idle);
        }
    }

    private void StopAllMovementAndRotation()
    {
        // 武器攻撃を停止
        _mainWeapon.SetAttack(false);

        // 攻撃中の回転を停止
        if (_isRotatingDuringAttack)
        {
            _playerController.StopTargetRotation();
            _isRotatingDuringAttack = false;
        }

        // 自動移動を停止
        if (_playerController.IsAutoMoving)
        {
            _playerController.CancelAutoMovement();
            _waitingForMovement = false;
        }

        // IKを強制リセット
        ForceResetIK();

        // アニメーション状態もリセット
        var animData = _mainWeapon.Data.WeaponAnimationData;
        _playerManager.AnimController.ResetAttack(animData.AttackTrigger, animData.AnimationType);
        _playerManager.AnimController.SetWeight(animData.AnimationLayer, 0);

        Debug.Log("必殺技開始：すべての動作を停止しました");
    }

    private void OnDisable()
    {
        _input.AttackAction.started -= Attack;
        _input.AttackAction.canceled -= Attack;
        _input.WeaponChangeAction.started -= WeaponChange;
        _input.ReloadAction.started -= Reload;
        _input.SpecialAction.started -= Special;

        // 無効化時に回転制御も停止
        if (_isRotatingDuringAttack)
        {
            _playerController.StopTargetRotation();
            _isRotatingDuringAttack = false;
        }

        // IKも強制リセット
        ForceResetIK();
    }
}