using Script.System.Ingame;
using UnityEngine;
using UnityEngine.InputSystem;
// TODO:単一責任の原則を守るため、機能の分離をする
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Character_B<PlayerData>
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerData _dataBase;
    [SerializeField] private GuardCollider _collider;
    [SerializeField] private float _rotateSpeed = 10;
    [SerializeField] private GhostSpawner _ghostSpawner;

    [Header("ダッシュ設定")]
    [SerializeField] private string[] _ignoreLayers;
    [SerializeField] private Vector3 _dashColliderSize = new Vector3(1f, 2f, 1f);
    [SerializeField] private float _dashSafetyMargin = 0.5f;
    [SerializeField] private float _dashPushBackDistance = 0.3f;
    [SerializeField] private int _rayCastHitSize = 10;
    private RaycastHit[] _dashHitCache;

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
    [SerializeField] private float _arriveThreshold = 1.5f;
    public float ArriveThreshold => _arriveThreshold; // PlayerAttackからアクセスするためpublic
    [SerializeField] private float _maxAutoMoveTime = 3f;

    [Header("Animation補間時間")]
    [SerializeField] private float _dampTime = 0.1f;
    private float _moveX;
    private float _moveY;

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
    // ダッシュ結果を構造体で管理
    private struct DashResult
    {
        public bool hasHit;
        public Vector3 targetPosition;
        public GameObject hitObject;
        public RaycastHit hitInfo;
    }
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

        _dashHitCache = new RaycastHit[_rayCastHitSize];

        _camera = Camera.main;

        Start_B();
    }

    private void Update()
    {
        // ポーズ中は何もしない
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

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

        // ダッシュ状態を解除
        _isDashed = false;

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
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

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
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

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
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

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
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }
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
        if (_gameManager.IsPaused || _gameManager.IsInEvent) return;
        if (_playerManager.IsState(PlayerState.SpecialAttack)) return;

        if (context.phase == InputActionPhase.Started && !_isDashed)
        {
            if (_isAutoMoving) return;
            if (!GaugeValueChange(-_data.DashValue)) return;

            ExecuteDash();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            _isBoost = false;
            UpdateFastEffect();
        }
    }

    private void ExecuteDash()
    {
        _isBoost = true;
        _data.DashTimer = 0;
        _dashStartPos = transform.position;

        // ダッシュ方向を決定
        Vector3 dashDirection = CalculateDashDirection();

        // BoxCastで障害物チェック
        DashResult result = PerformDashBoxCast(dashDirection);

        // 結果を適用
        ApplyDashResult(result);

        _isDashed = true;
        UpdateFastEffect();
    }

    /// <summary>
    /// ダッシュ方向を計算
    /// </summary>
    private Vector3 CalculateDashDirection()
    {
        Vector3 direction;

        if (_velocity != Vector2.zero)
        {
            // 入力がある場合はその方向
            direction = _moveDir.normalized;
        }
        else
        {
            // 入力がない場合はカメラの正面
            direction = _camForward.normalized;
        }

        // Y軸を0にして水平方向のみにする
        direction.y = 0;
        return direction.normalized;
    }

    /// <summary>
    /// BoxCastNonAllocでダッシュ経路の障害物を検出
    /// </summary>
    private DashResult PerformDashBoxCast(Vector3 direction)
    {
        DashResult result = new DashResult();

        Vector3 startPosition = GetTargetCenter().position;
        Vector3 halfExtents = _dashColliderSize * 0.5f;
        float maxDistance = _data.DashDistance;

        // ignoreLayersを反転して「検出すべきレイヤー」のマスクを作成
        int ignoreMask = LayerMask.GetMask(_ignoreLayers);
        int detectMask = ~ignoreMask;

        // BoxCastNonAllocで検出
        int hitCount = Physics.BoxCastNonAlloc(
            center: startPosition,
            halfExtents: halfExtents,
            direction: direction,
            results: _dashHitCache,
            orientation: transform.rotation,
            maxDistance: maxDistance,
            layerMask: detectMask
        );

        if (hitCount > 0)
        {
            // 最も近い障害物を見つける
            RaycastHit closestHit = _dashHitCache[0];
            float minDistance = _dashHitCache[0].distance;

            for (int i = 1; i < hitCount; i++)
            {
                if (_dashHitCache[i].distance < minDistance)
                {
                    minDistance = _dashHitCache[i].distance;
                    closestHit = _dashHitCache[i];
                }
            }

            result.hasHit = true;
            result.hitInfo = closestHit;
            result.hitObject = closestHit.collider.gameObject;
            result.targetPosition = CalculateSafePosition(closestHit, direction);

            string hitLayerName = LayerMask.LayerToName(closestHit.collider.gameObject.layer);
            Debug.Log($"[Dash] 障害物検出: {closestHit.collider.name} " +
                     $"(レイヤー: {hitLayerName}, 距離: {closestHit.distance:F2}m, " +
                     $"検出数: {hitCount}{closestHit.collider.name} => {closestHit.point})");

            if (hitCount >= _dashHitCache.Length)
            {
                Debug.LogWarning($"[Dash] 検出数がキャッシュサイズ({_dashHitCache.Length})を超えています。");
            }
        }
        else
        {
            result.hasHit = false;
            result.targetPosition = transform.position + direction * _data.DashDistance;
            result.hitObject = null;

            Debug.Log($"[Dash] 障害物なし。目標距離: {_data.DashDistance:F2}m");
        }

        return result;
    }

    /// <summary>
    /// 衝突位置から安全な停止位置を計算
    /// </summary>
    private Vector3 CalculateSafePosition(RaycastHit hit, Vector3 dashDirection)
    {
        const float MIN_DISTANCE_THRESHOLD = 0.1f;

        Vector3 safePosition; // ワールド座標で計算

        // 0距離ヒット（既に障害物に接触している、またはhit.pointが不正）の場合
        if (hit.distance <= MIN_DISTANCE_THRESHOLD || hit.point == Vector3.zero)
        {
            Debug.LogWarning($"[Dash] 0距離ヒット検出: {hit.collider.name}, distance={hit.distance:F3}, point={hit.point}");

            // hit.pointが信頼できないので、Collider.ClosestPointを使用
            Vector3 playerCenter = GetTargetCenter().position;
            Vector3 closestPoint = hit.collider.ClosestPoint(playerCenter);

            // プレイヤーから最近接点への方向を計算
            Vector3 pushDirection = (playerCenter - closestPoint).normalized;
            pushDirection.y = 0; // 水平方向のみ

            // 押し出し方向が取れない場合はダッシュ方向の逆を使用
            if (pushDirection.magnitude < 0.1f)
            {
                pushDirection = -dashDirection;
                Debug.LogWarning("[Dash] 押し出し方向が計算できないため、ダッシュ方向の逆を使用");
            }
            else
            {
                pushDirection.Normalize();
            }

            float pushDistance = _dashSafetyMargin + _dashPushBackDistance;
            safePosition = transform.position + pushDirection * pushDistance;

            Debug.Log($"[Dash] 0距離対応: closestPoint={closestPoint}, pushDir={pushDirection}, safePos={safePosition}");
        }
        else
        {
            // 通常のヒット：衝突点から安全距離だけ手前に戻る

            // 押し出し方向の決定
            Vector3 pushDirection;

            // 法線のY成分が大きい場合は斜面
            if (Mathf.Abs(hit.normal.y) > 0.3f)
            {
                // 斜面の場合：法線の水平成分を使用
                pushDirection = new Vector3(hit.normal.x, 0, hit.normal.z).normalized;
                Debug.Log($"[Dash] 斜面検出: 法線Y={hit.normal.y:F2}");
            }
            else
            {
                // 垂直な壁の場合：ダッシュ方向の逆を使用
                pushDirection = -dashDirection;
            }

            float safeDistance = _dashSafetyMargin + _dashPushBackDistance;

            // hit.pointは既にワールド座標
            safePosition = hit.point + pushDirection * safeDistance;

            Debug.Log($"[Dash] 通常ヒット: hit.point={hit.point}, pushDir={pushDirection}, safePos={safePosition}");
        }

        // Y座標は現在の高さを維持
        safePosition.y = transform.position.y;

        return safePosition; // ワールド座標を返す
    }

    /// <summary>
    /// ダッシュ結果を適用
    /// </summary>
    private void ApplyDashResult(DashResult result)
    {
        _dashTargetPos = result.targetPosition;
        _newPos = result.targetPosition;
        _conflictObj = result.hitObject;
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
            UpdateFastEffect();
        }
    }

    /// <summary>
    /// 残像とカメラエフェクトを現在の状態に応じて更新
    /// </summary>
    private void UpdateFastEffect()
    {
        // エフェクトを表示すべき条件
        bool shouldShowEffect = false;

        // ダッシュ中は常に表示
        if (_isDashed)
        {
            shouldShowEffect = true;
        }
        // ブースト中かつ入力がある時のみ表示
        else if (_isBoost && _velocity != Vector2.zero)
        {
            shouldShowEffect = true;
        }
        // 自動移動中は常に表示
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

    private void OnGuard(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent || _playerManager.IsState(PlayerState.SpecialAttack)) { return; }

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

    private void GuardVisibleChange()
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
    private void OnGuardHit(Collider other)
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }

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
        Vector2 target = new Vector2(_velocity.x, _velocity.y);

        // Unityの内部補間に近い指数補間係数
        float t = 1f - Mathf.Exp(-Time.deltaTime / _dampTime);

        _moveX = Mathf.Lerp(_moveX, target.x, t);
        _moveY = Mathf.Lerp(_moveY, target.y, t);

        Vector2 nextValue = new Vector2(_moveX, _moveY);

        // 現在Animatorに設定されている値
        var anim = _playerManager.AnimController;
        Vector2 currentValue = new Vector2(
            anim.GetFloat("MoveX"),
            anim.GetFloat("MoveY")
        );

        // Lerpなどで値が微妙に動き続けてると、
        // Animatorが無限に内部時間を積み上げてしまうので
        // 一定以上の変化がある場合のみAnimatorを更新
        const float threshold = 0.001f;
        if (Vector2.Distance(currentValue, nextValue) > threshold)
        {
            anim.SetFloat("MoveX", nextValue.x);
            anim.SetFloat("MoveY", nextValue.y);
        }
    }


    private void OnDisable()
    {
        RemoveAction();
    }

    private void AddAction()
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

    private void RemoveAction()
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
        if (!_isDashed && !Application.isPlaying) return;

        Vector3 direction = CalculateDashDirection();
        Vector3 startPosition = GetTargetCenter().position;
        Vector3 halfExtents = _dashColliderSize * 0.5f;

        // BoxCastの可視化
        Gizmos.color = Color.cyan;

        // 開始位置のボックス
        Gizmos.matrix = Matrix4x4.TRS(startPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _dashColliderSize);

        // 終了位置のボックス
        Vector3 endPosition = startPosition + direction * _data.DashDistance;
        Gizmos.matrix = Matrix4x4.TRS(endPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _dashColliderSize);

        // 経路を線で表示
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);

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