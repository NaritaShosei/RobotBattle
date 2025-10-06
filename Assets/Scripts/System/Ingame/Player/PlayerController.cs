using Script.System.Ingame;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Character_B<PlayerData>
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerData _dataBase;
    [SerializeField] private GuardCollider _collider;
    [SerializeField] private float _rotateSpeed = 10;
    [SerializeField] private GhostSpawner _ghostSpawner;

    [Header("ダッシュ時に判定しないレイヤー")]
    [SerializeField] private string[] _ignoreLayers;

    private Rigidbody _rb;
    private InputManager _input;

    /// <summary>
    /// 衝突中のオブジェクト
    /// </summary>
    private GameObject _conflictObj;

    /// <summary>
    /// 入力情報の保持
    /// </summary>
    [Tooltip("移動の入力ベクトル")] private Vector2 _velocity;
    private Vector3 _camForward;
    private Vector3 _camRight;
    private Vector3 _moveDir;
    private Vector3 _dashStartPos;
    private Vector3 _dashTargetPos;

    /// <summary>
    /// 線形補完によって出されたダッシュ時の座標
    /// </summary>
    private Vector3 _newPos;

    /// <summary>
    /// ブースト時のスピードを線形補完にするための変数
    /// </summary>
    private float _currentSpeed;

    private bool _isJumped;
    private bool _isDashed;
    private bool _isBoost;
    private bool _isGuard;

    // 自動移動関連
    [Header("自動移動設定")]
    [SerializeField] public float _arriveThreshold = 1.5f;
    public float ArriveThreshold => _arriveThreshold; // PlayerAttackからアクセスするためpublic
    [SerializeField] private float _maxAutoMoveTime = 3f;

    [Header("Animation補間時間")]
    [SerializeField] private float _dampTime = 0.1f;

    private float _autoMoveSpeed;

    private bool _isAutoMoving = false;
    private Transform _autoMoveTarget;
    private float _autoMoveTimer = 0f;
    private System.Action _onAutoMoveComplete;
    private System.Action _onAutoMoveCanceled;

    // 攻撃中の回転制御用
    private bool _shouldRotateToTarget = false;
    private Vector3 _attackToRotationTargetDir;

    private HPGaugePresenter _healthPresenter;
    private GaugePresenter _gaugePresenter;
    private IngameManager _gameManager;
    private CameraManager _cameraManager;
    private Camera _camera;

    void Start()
    {
        _input = ServiceLocator.Get<InputManager>();
        AddAction();

        _gameManager = ServiceLocator.Get<IngameManager>();

        _cameraManager = ServiceLocator.Get<CameraManager>();
        _rb = GetComponent<Rigidbody>();

        Initialize(_dataBase);

        _currentSpeed = _data.NormalSpeed;
        _healthPresenter = new HPGaugePresenter(ServiceLocator.Get<GameUIManager>().HPGaugeView);
        _healthPresenter.Initialize(_data.Health);
        _gaugePresenter = new GaugePresenter(ServiceLocator.Get<GameUIManager>().GaugeView);
        _gaugePresenter.Initialize(_data.Gauge);
        _collider.GuardVisible(false);

        _camera = Camera.main;

        Start_B();
    }

    private void Update()
    {
        // ポーズ中は何もしない
        if (_gameManager.IsPaused) { return; }

        // 必殺技中は移動・回転を完全停止
        if (_playerManager.IsState(PlayerState.SpecialAttack))
        {
            StopAllMovement(false);
            StopTargetRotation();
            return;
        }

        GuardVisibleChange();

        if (_playerManager.IsState(PlayerState.Dead))
        {
            _isBoost = false;
            _isDashed = false;
            _isGuard = false;
            _isJumped = false;
            StopAllMovement(true);
            StopTargetRotation();
            return;
        }

        // 自動移動の処理
        if (_isAutoMoving)
        {
            HandleAutoMovement();
            return;
        }

        // ダッシュとジャンプ両方していなかったらゲージを回復
        if (!_isDashed && !_isJumped)
        {
            GaugeValueChange(_data.RecoveryValue * Time.deltaTime);
        }

        // ジャンプ中にゲージがなくなったらジャンプを解除
        if (_isJumped)
        {
            if (!GaugeValueChange(-_data.JumpValue * Time.deltaTime))
            {
                _isJumped = false;
            }
        }

        // ブースト中にゲージがなくなったらブーストを解除
        if (_isBoost)
        {
            // 入力がなかったらゲージを減らさない
            if (_velocity != Vector2.zero)
            {
                if (!GaugeValueChange(-_data.DashValue * Time.deltaTime))
                {
                    _isBoost = false;
                    UpdateFastEffect(); // 追加
                }
            }
            else
            {
                // ブースト中に入力が止まったら残像とカメラを停止
                UpdateFastEffect(); // 追加
            }
        }

        // ダッシュフラグが立っていたらダッシュ
        if (_isDashed)
        {
            Dash();
        }
        // ダッシュフラグがおりていたら通常の移動
        else if (!_isDashed)
        {
            Move(_isBoost ? _data.BoostSpeed : _data.NormalSpeed);
        }

        HandleRotation();
        SetMoveAnimParam();
    }


    /// <summary>
    /// 回転処理を統合
    /// </summary>
    private void HandleRotation()
    {
        if (_playerManager.IsState(PlayerState.SpecialAttack)) return;

        Vector3 targetDirection;

        // 目標への回転が有効な場合
        if (_shouldRotateToTarget && _attackToRotationTargetDir != Vector3.zero)
        {
            Vector3 directionToTarget = _attackToRotationTargetDir.normalized;
            targetDirection = directionToTarget;
        }
        else
        {
            // 通常の回転（カメラ方向）
            var cam = _camera.transform.forward;
            cam.y = 0;
            cam.Normalize();
            targetDirection = cam;
        }

        // 回転の補間
        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 攻撃中の目標回転を開始
    /// </summary>
    /// <param name="target">回転する目標</param>
    public void StartTargetRotation()
    {
        _shouldRotateToTarget = true;

        // 追尾中は重力を切る
        _rb.useGravity = false;
        Debug.Log($"目標回転開始");
    }

    /// <summary>
    /// 目標回転を停止
    /// </summary>
    public void StopTargetRotation()
    {
        _shouldRotateToTarget = false;
        _attackToRotationTargetDir = Vector3.zero;

        var forward = transform.forward;
        forward.y = 0;
        transform.forward = forward;

        // 追尾終了時に重力をかけなおす
        _rb.useGravity = true;
        Debug.Log("目標回転停止");
    }

    /// <summary>
    /// 自動移動の処理
    /// </summary>
    private void HandleAutoMovement()
    {
        _autoMoveTimer += Time.deltaTime;

        // タイムアウトチェック
        if (_autoMoveTimer >= _maxAutoMoveTime)
        {
            Debug.LogWarning("自動移動がタイムアウトしました");
            CancelAutoMovement();
            return;
        }

        // TargetCenterを基準とした距離チェック
        Vector3 currentCenter = GetTargetCenter().position;
        float distanceToTarget = Vector3.Distance(currentCenter, _autoMoveTarget.position);

        // 目標への方向を保持
        Vector3 direction = (_autoMoveTarget.position - currentCenter).normalized;

        if (distanceToTarget <= ArriveThreshold)
        {
            // 目標地点に到達
            Debug.Log("目標地点に到達しました");
            CompleteAutoMovement();
            _attackToRotationTargetDir = direction;
        }
        else
        {

            // 目標地点に向かって移動
            // 移動方向を設定（既存のMove関数を利用するため）
            Vector3 moveDirection = direction * _autoMoveSpeed;

            _rb.linearVelocity = moveDirection;

            // プレイヤーを移動方向に向ける
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// 自動移動を開始
    /// </summary>
    /// <param name="target">移動先の標的</param>
    /// <param name="onComplete">到達時のコールバック</param>
    /// <param name="onCanceled">キャンセル時のコールバック</param>
    public void StartAutoMovement(Transform target, float speed, System.Action onComplete = null, System.Action onCanceled = null)
    {
        if (_playerManager.IsState(PlayerState.SpecialAttack)) return;

        if (_isAutoMoving)
        {
            Debug.LogWarning("既に自動移動中です");
            return;
        }

        _autoMoveTarget = target;
        _autoMoveSpeed = speed;
        _isAutoMoving = true;
        _autoMoveTimer = 0f;
        _onAutoMoveComplete = onComplete;
        _onAutoMoveCanceled = onCanceled;

        _playerManager.SetState(PlayerState.MovingToTarget);

        // 自動移動開始時に残像とカメラエフェクトを開始 - 追加
        _ghostSpawner.StartSpawning();
        _cameraManager.SetFastMode(true);

        Debug.Log($"自動移動開始: 目標地点 {target}, 距離: {Vector3.Distance(transform.position, target.position):F2}m");
    }

    /// <summary>
    /// 自動移動の完了処理
    /// </summary>
    private void CompleteAutoMovement()
    {
        Debug.Log("自動移動完了");

        StopMovement();

        var onComplete = _onAutoMoveComplete;
        ResetAutoMovement();

        onComplete?.Invoke();

        UpdateFastEffect(); // StopFast()から変更
    }

    /// <summary>
    /// 自動移動のキャンセル処理
    /// </summary>
    public void CancelAutoMovement()
    {
        if (!_isAutoMoving) return;

        Debug.Log("自動移動をキャンセルしました");

        StopMovement();

        var onCanceled = _onAutoMoveCanceled;
        ResetAutoMovement();

        onCanceled?.Invoke();

        UpdateFastEffect(); // 追加
    }

    /// <summary>
    /// 自動移動を強制停止（外部から呼び出し用）
    /// </summary>
    public void StopAutoMovement()
    {
        if (_isAutoMoving)
        {
            CancelAutoMovement();
        }
    }

    /// <summary>
    /// 移動を停止
    /// </summary>
    private void StopMovement()
    {
        Vector3 velocity = _rb.linearVelocity;
        velocity.x = 0;
        velocity.z = 0;
        _rb.linearVelocity = velocity;

        // 入力状態もリセット
        _velocity = Vector2.zero;
        _moveDir = Vector3.zero;
    }
    /// <summary>
    /// すべての移動要素を停止
    /// </summary>
    private void StopAllMovement(bool inputKill)
    {
        // 入力をリセット
        if (inputKill)
        {
            _velocity = Vector2.zero;
            _moveDir = Vector3.zero;
        }

        // 各種フラグを停止
        _isBoost = false;
        _isDashed = false;
        _isJumped = false;

        // 自動移動も停止
        StopAutoMovement();

        // 残像の停止処理
        UpdateFastEffect();
    }

    /// <summary>
    /// 自動移動関連の変数をリセット
    /// </summary>
    private void ResetAutoMovement()
    {
        _isAutoMoving = false;
        _autoMoveTimer = 0f;
        _onAutoMoveComplete = null;
        _onAutoMoveCanceled = null;

        // Idle状態に戻す（他の状態でなければ）
        if (_playerManager.IsState(PlayerState.MovingToTarget))
        {
            _playerManager.SetState(PlayerState.Idle);
        }

        // 念のため入力状態を再度リセット
        _velocity = Vector2.zero;
        _moveDir = Vector3.zero;
    }

    /// <summary>
    /// 自動移動中かどうか
    /// </summary>
    public bool IsAutoMoving => _isAutoMoving;

    private void FixedUpdate()
    {
        if (_gameManager.IsPaused) { return; }

        //AddForceなどはFixedUpdateで

        // 重力
        float fallSpeed = _rb.useGravity ? _data.FallSpeed : 0f;
        _rb.AddForce(Vector3.down * fallSpeed, ForceMode.Force);

        if (_playerManager.IsState(PlayerState.SpecialAttack))
        {
            // 必殺技中は水平移動を完全に停止
            Vector3 currentVelocity = _rb.linearVelocity;
            _rb.linearVelocity = new Vector3(0, currentVelocity.y, 0);
            return;
        }

        if (_isDashed)
        {
            _rb.MovePosition(_newPos);
        }
        if (_isJumped)
        {
            _rb.AddForce(Vector3.up * _data.FloatPower, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ダッシュ中に床以外のオブジェクトにぶつかったらダッシュの目的地を今いる座標に
        if (!collision.gameObject.CompareTag("Plane"))
        {
            if (!_conflictObj && _isDashed)
            {
                _conflictObj = collision.gameObject;
                _newPos = transform.position;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //ぶつかっているオブジェクトを解除
        if (!collision.gameObject.CompareTag("Plane"))
        {
            if (_conflictObj)
            {
                _conflictObj = null;
            }
        }
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        if (_isAutoMoving) return;

        var input = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Performed)
        {
            _velocity = input;

            if (_velocity.magnitude > 1)
            {
                _velocity = _velocity.normalized;
            }
            // Boost中に入力が復活したら残像とカメラを再開
            UpdateFastEffect(); // 追加
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _velocity = Vector2.zero;
            UpdateFastEffect(); // 追加
        }
    }
    private void Move(float speed)
    {
        if (_gameManager.IsPaused) { return; }

        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        //カメラのYを無視した正面と右の取得
        _camForward = _camera.transform.forward;
        _camRight = _camera.transform.right;
        _camForward.y = _camRight.y = 0;
        var forwardDir = _camForward.normalized;
        var rightDir = _camRight.normalized;

        //カメラの正面に合わせた移動方向
        _moveDir = forwardDir * _velocity.y + rightDir * _velocity.x;

        //velocityに代入
        var vel = _moveDir * speed;
        var currentVel = _rb.linearVelocity;
        vel.y = currentVel.y;
        _rb.linearVelocity = vel;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }
        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        _isJumped = context.phase == InputActionPhase.Started;

        // 自動移動中はジャンプを無効
        if (_isAutoMoving) return;

        //ボタンを押した瞬間強めに上昇
        if (_isJumped)
        {
            if (!GaugeValueChange(-_data.JumpValue)) return;
            //マジックナンバー
            _isJumped = true;
            _rb.AddForce(Vector3.up * _data.JumpPower * 5, ForceMode.Impulse);
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }
        if (_playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        if (context.phase == InputActionPhase.Started && !_isDashed)
        {
            if (_isAutoMoving) return;

            if (!GaugeValueChange(-_data.DashValue)) return;

            _isBoost = true;
            _data.DashTimer = 0;
            _dashStartPos = transform.position;

            Vector3 moveVel = _velocity != Vector2.zero ? _moveDir : _camForward;
            Vector3 moveDir = moveVel.normalized;

            var rayCastDis = 8;
            _isDashed = true;

            int layerMask = ~LayerMask.GetMask(_ignoreLayers);

            if (Physics.Raycast(GetTargetCenter().position, moveDir, out RaycastHit hit, rayCastDis, layerMask))
            {
                Debug.Log(hit.collider.name);
                var dir = (transform.position - hit.point).normalized;
                var newPos = hit.point + dir * 10;
                newPos.y = transform.position.y;
                _newPos = newPos;
                _conflictObj = hit.collider.gameObject;
            }
            else
            {
                _dashTargetPos = transform.position + moveDir * _data.DashDistance;
            }

            UpdateFastEffect(); // 追加
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            _isBoost = false;
            UpdateFastEffect(); // StopFast()から変更
        }
    }

    private void Dash()
    {
        _data.DashTimer += Time.deltaTime;

        var t = Mathf.Clamp01(_data.DashTimer / _data.DashTime);

        if (!_conflictObj)
        {
            _newPos = Vector3.Lerp(_dashStartPos, _dashTargetPos, t);
        }

        if (t >= 1)
        {
            _conflictObj = null;
            _isDashed = false;
            UpdateFastEffect(); // StopFast()から変更
        }
    }

    // 新しいメソッド: 残像とカメラエフェクトの統合管理
    /// <summary>
    /// 残像とカメラエフェクトを現在の状態に応じて更新
    /// </summary>
    private void UpdateFastEffect()
    {
        // エフェクトを表示すべき条件
        bool shouldShowEffect = false;

        // 1. ダッシュ中は常に表示
        if (_isDashed)
        {
            shouldShowEffect = true;
        }
        // 2. ブースト中かつ入力がある時のみ表示
        else if (_isBoost && _velocity != Vector2.zero)
        {
            shouldShowEffect = true;
        }
        // 3. 自動移動中は常に表示
        else if (_isAutoMoving)
        {
            shouldShowEffect = true;
        }

        // 状態に応じてエフェクトを制御
        if (shouldShowEffect)
        {
            _ghostSpawner.StartSpawning();
            _cameraManager.SetFastMode(true);
        }
        else
        {
            _ghostSpawner.StopSpawning();
            _cameraManager.SetFastMode(false);
        }
    }


    void OnGuard(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused || _playerManager.IsState(PlayerState.SpecialAttack)) { return; }

        // 自動移動中はガードを無効
        if (_isAutoMoving) return;

        if (context.phase == InputActionPhase.Started)
        {
            //ゲージが残ってるかつIdle状態のときのみガード可能
            if (_data.Gauge >= _data.GuardMinValue && _playerManager.IsState(PlayerState.Idle))
            {
                Debug.Log("ガード開始");
                _isGuard = true;
                _playerManager.SetState(PlayerState.Guard);
            }
        }

        if (context.phase == InputActionPhase.Canceled && _playerManager.IsState(PlayerState.Guard))
        {
            Debug.Log("ガード終了");
            _playerManager.SetState(PlayerState.Idle);
            _isGuard = false;
        }
    }

    void GuardVisibleChange()
    {
        if (_isGuard)
        {
            _collider.GuardVisible(true);
        }
        else if (!_isGuard)
        {
            _collider.GuardVisible(false);
        }
    }

    /// <summary>
    /// ガード成功時にゲージを減らす
    /// </summary>
    /// <param name="other"></param>
    void OnGuardHit(Collider other)
    {
        if (_gameManager.IsPaused) { return; }

        if (other.TryGetComponent(out Bullet_B bullet))
        {
            if (!GaugeValueChange(-bullet.GuardBreakValue))
            {
                _isGuard = false;
            }
        }
    }

    public override void HitDamage(IWeapon other)
    {
        if (_isGuard) return;
        base.HitDamage(other);
    }

    protected override void Dead()
    {
        base.Dead();
        _playerManager.SetState(PlayerState.Dead);
    }

    protected override void OnHealthChanged(float health)
    {
        _healthPresenter.GaugeUpdate(health);
    }

    protected override void OnGaugeChanged(float value)
    {
        _gaugePresenter.GaugeUpdate(value);
    }

    private void SetMoveAnimParam()
    {
        _playerManager.AnimController.SetFloat("MoveX", _velocity.x, _dampTime);
        _playerManager.AnimController.SetFloat("MoveY", _velocity.y, _dampTime);
    }

    private void OnDisable()
    {
        RemoveAction();
    }

    void AddAction()
    {
        Debug.Log("InputEventが登録されました");
        _collider.OnTriggerEnterEvent += OnGuardHit;
        _input.MoveAction.performed += OnMoveInput;
        _input.MoveAction.canceled += OnMoveInput;
        _input.JumpAction.started += OnJump;
        _input.JumpAction.canceled += OnJump;
        _input.DashAction.started += OnDash;
        _input.DashAction.canceled += OnDash;
        _input.GuardAction.started += OnGuard;
        _input.GuardAction.canceled += OnGuard;
    }

    void RemoveAction()
    {
        Debug.Log("InputEventが破棄されました");
        _collider.OnTriggerEnterEvent -= OnGuardHit;
        _input.MoveAction.performed -= OnMoveInput;
        _input.MoveAction.canceled -= OnMoveInput;
        _input.JumpAction.started -= OnJump;
        _input.JumpAction.canceled -= OnJump;
        _input.DashAction.started -= OnDash;
        _input.DashAction.canceled -= OnDash;
        _input.GuardAction.started -= OnGuard;
        _input.GuardAction.canceled -= OnGuard;
    }

    // デバッグ用の可視化
    private void OnDrawGizmos()
    {
        if (_isAutoMoving)
        {
            // 移動目標を赤い球で表示
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_autoMoveTarget.position, 0.5f);

            // 現在位置から目標までの線
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _autoMoveTarget.position);

            // 到達判定範囲
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_autoMoveTarget.position, _arriveThreshold);
        }

        // 回転目標の可視化
        if (_shouldRotateToTarget && _attackToRotationTargetDir != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_attackToRotationTargetDir, 0.3f);
            Gizmos.DrawLine(transform.position, _attackToRotationTargetDir);
        }
    }
}