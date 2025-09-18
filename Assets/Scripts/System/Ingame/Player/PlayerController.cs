using Script.System.Ingame;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Character_B<PlayerData>
{
    [SerializeField] PlayerManager _playerManager;
    [SerializeField] PlayerData _dataBase;
    [SerializeField] GuardCollider _collider;
    [SerializeField] float _rotateSpeed = 10;

    Rigidbody _rb;
    InputManager _input;

    /// <summary>
    /// 衝突中のオブジェクト
    /// </summary>
    GameObject _conflictObj;

    /// <summary>
    /// 入力情報の保持
    /// </summary>
    Vector2 _velocity;
    Vector3 _camForward;
    Vector3 _camRight;
    Vector3 _moveDir;
    Vector3 _dashStartPos;
    Vector3 _dashTargetPos;

    /// <summary>
    /// 線形補完によって出されたダッシュ時の座標
    /// </summary>
    Vector3 _newPos;

    /// <summary>
    /// ブースト時のスピードを線形補完にするための変数
    /// </summary>
    float _currentSpeed;

    bool _isJumped;
    bool _isDashed;
    bool _isBoost;
    bool _isGuard;

    // 自動移動関連
    [Header("自動移動設定")]
    [SerializeField] public float _arriveThreshold = 1.5f;
    public float ArriveThreshold => _arriveThreshold; // PlayerAttackからアクセスするためpublic
    [SerializeField] private float _maxAutoMoveTime = 3f;

    private bool _isAutoMoving = false;
    private Vector3 _autoMoveTarget;
    private float _autoMoveTimer = 0f;
    private System.Action _onAutoMoveComplete;
    private System.Action _onAutoMoveCanceled;

    HPGaugePresenter _healthPresenter;
    GaugePresenter _gaugePresenter;
    IngameManager _gameManager;
    private Camera _camera;

    void Start()
    {
        _input = ServiceLocator.Get<InputManager>();
        AddAction();

        _gameManager = ServiceLocator.Get<IngameManager>();
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

    void Update()
    {
        //ポーズ中は何もしない
        if (_gameManager.IsPaused) { return; }

        //ガードのコライダーのon・off
        GuardVisibleChange();

        //Playerが死亡したらすべてのフラグをおろして処理を抜ける
        if (_playerManager.IsState(PlayerState.Dead))
        {
            _isBoost = false;
            _isDashed = false;
            _isGuard = false;
            _isJumped = false;
            StopAutoMovement(); // 自動移動も停止
            return;
        }

        // 自動移動の処理
        if (_isAutoMoving)
        {
            HandleAutoMovement();
            return; // 自動移動中は通常の処理をスキップ
        }

        //ダッシュとジャンプ両方していなかったらゲージを回復
        if (!_isDashed && !_isJumped)
        {
            GaugeValueChange(_data.RecoveryValue * Time.deltaTime);
        }

        //ジャンプ中にゲージがなくなったらジャンプを解除
        if (_isJumped)
        {
            if (!GaugeValueChange(-_data.JumpValue * Time.deltaTime))
            {
                _isJumped = false;
            }
        }

        //ブースト中にゲージがなくなったらブーストを解除
        if (_isBoost)
        {
            //入力がなかったらゲージをへらさない
            if (_velocity != Vector2.zero)
            {
                if (!GaugeValueChange(-_data.DashValue * Time.deltaTime))
                {
                    _isBoost = false;
                }
            }
        }

        //ダッシュフラグが立っていたらダッシュ
        if (_isDashed)
        {
            Dash();
        }
        //ダッシュフラグがおりていたら通常の移動
        else if (!_isDashed)
        {
            Move(_isBoost ? _data.BoostSpeed : _data.NormalSpeed);
        }

        var cam = _camera.transform.forward;
        cam.y = 0;
        cam.Normalize();

        // 目標方向を更新
        var _targetDir = cam;

        // 回転の補間
        if (_targetDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(_targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
        }
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
        float distanceToTarget = Vector3.Distance(currentCenter, _autoMoveTarget);

        if (distanceToTarget <= ArriveThreshold)
        {
            // 目標地点に到達
            Debug.Log("目標地点に到達しました");
            CompleteAutoMovement();
        }
        else
        {
            // 目標地点に向かって移動
            Vector3 direction = (_autoMoveTarget - currentCenter).normalized;

            // 移動方向を設定（既存のMove関数を利用するため）
            Vector3 moveDirection = direction * _data.BoostSpeed;

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
    /// <param name="targetPosition">移動先の座標</param>
    /// <param name="onComplete">到達時のコールバック</param>
    /// <param name="onCanceled">キャンセル時のコールバック</param>
    public void StartAutoMovement(Vector3 targetPosition, System.Action onComplete = null, System.Action onCanceled = null)
    {
        if (_isAutoMoving)
        {
            Debug.LogWarning("既に自動移動中です");
            return;
        }

        _autoMoveTarget = targetPosition;
        _isAutoMoving = true;
        _autoMoveTimer = 0f;
        _onAutoMoveComplete = onComplete;
        _onAutoMoveCanceled = onCanceled;

        _playerManager.SetState(PlayerState.MovingToTarget);

        Debug.Log($"自動移動開始: 目標地点 {targetPosition}, 距離: {Vector3.Distance(transform.position, targetPosition):F2}m");
    }

    /// <summary>
    /// 自動移動の完了処理
    /// </summary>
    private void CompleteAutoMovement()
    {
        Debug.Log("自動移動完了");

        // 移動を停止
        StopMovement();

        var onComplete = _onAutoMoveComplete;
        ResetAutoMovement();

        // コールバック実行
        onComplete?.Invoke();
    }

    /// <summary>
    /// 自動移動のキャンセル処理
    /// </summary>
    public void CancelAutoMovement()
    {
        if (!_isAutoMoving) return;

        Debug.Log("自動移動をキャンセルしました");

        // 移動を停止
        StopMovement();

        // 物理的な速度を確実に停止
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);

        var onCanceled = _onAutoMoveCanceled;
        ResetAutoMovement();

        // コールバック実行
        onCanceled?.Invoke();
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

        // 入力状態もリセット（追加）
        _velocity = Vector2.zero;
        _moveDir = Vector3.zero;
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

    /// <summary>
    /// 自動移動の進捗（0.0～1.0）
    /// </summary>
    public float AutoMovementProgress
    {
        get
        {
            if (!_isAutoMoving) return 0f;
            return Mathf.Clamp01(_autoMoveTimer / _maxAutoMoveTime);
        }
    }

    private void FixedUpdate()
    {
        if (_gameManager.IsPaused) { return; }

        //AddForceなどはFixedUpdateで
        _rb.AddForce(Vector3.down * _data.FallSpeed, ForceMode.Force);

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

    void OnMoveInput(InputAction.CallbackContext context)
    {
        //移動の入力感知
        if (_gameManager.IsPaused) { return; }

        // 自動移動中は手動入力を無視
        if (_isAutoMoving) return;

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

    void Move(float speed)
    {
        if (_gameManager.IsPaused) { return; }

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

    void OnJump(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        // 自動移動中はジャンプを無効
        if (_isAutoMoving) return;

        //ボタンを押した瞬間強めに上昇
        if (context.phase == InputActionPhase.Started)
        {
            if (!GaugeValueChange(-_data.JumpValue)) return;
            //マジックナンバー
            _isJumped = true;
            _rb.AddForce(Vector3.up * _data.JumpPower * 5, ForceMode.Impulse);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isJumped = false;
        }
    }

    void OnDash(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

        // 自動移動中はダッシュを無効
        if (_isAutoMoving) return;

        if (context.phase == InputActionPhase.Started && !_isDashed)
        {
            if (!GaugeValueChange(-_data.DashValue)) return;

            //線形補間のstartの登録
            _isBoost = true;
            _data.DashTimer = 0;
            _dashStartPos = transform.position;

            //入力がなかったらカメラの正面方向
            Vector3 moveVel = _velocity != Vector2.zero ? _moveDir : _camForward;
            Vector3 moveDir = moveVel.normalized;

            //マジックナンバー
            var rayCastDis = 8;
            _isDashed = true;

            // レイヤーを除外
            int layerMask = ~LayerMask.GetMask("Weapon");

            // Raycastを飛ばす
            if (Physics.Raycast(GetTargetCenter().position, moveDir, out RaycastHit hit, rayCastDis, layerMask))
            {
                //hitしたらなにかにぶつかっているのでダッシュしない
                var dir = (transform.position - hit.point).normalized;
                //マジックナンバー
                var newPos = hit.point + dir * 10;
                newPos.y = transform.position.y;
                _newPos = newPos;
                _conflictObj = hit.collider.gameObject;
            }
            else
            {
                //線形補間のtargetの設定
                _dashTargetPos = transform.position + moveDir * _data.DashDistance;
            }
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            _isBoost = false;
        }
    }

    void Dash()
    {
        _data.DashTimer += Time.deltaTime;

        var t = Mathf.Clamp01(_data.DashTimer / _data.DashTime);

        //なにもぶつかっていなければダッシュ
        if (!_conflictObj)
        {
            //線形補間
            _newPos = Vector3.Lerp(_dashStartPos, _dashTargetPos, t);
        }

        if (t >= 1)
        {
            _conflictObj = null;
            _isDashed = false;
        }
    }

    void OnGuard(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }

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
            Gizmos.DrawWireSphere(_autoMoveTarget, 0.5f);

            // 現在位置から目標までの線
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _autoMoveTarget);

            // 到達判定範囲
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_autoMoveTarget, _arriveThreshold);
        }
    }
}