﻿using Script.System.Ingame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Character_B<PlayerData>
{
    [SerializeField]
    PlayerManager _playerManager;
    [SerializeField]
    PlayerData _dataBase;
    [SerializeField]
    GuardCollider _collider;
    [SerializeField]
    float _rotateSpeed = 10;
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

    HPGaugePresenter _healthPresenter;
    GaugePresenter _gaugePresenter;
    InGameManager _gameManager;
    private void OnEnable()
    {
        _input = ServiceLocator.Get<InputManager>();
        AddAction();
    }

    void Start()
    {
        _gameManager = ServiceLocator.Get<InGameManager>();
        _rb = GetComponent<Rigidbody>();
        Initialize(_dataBase);
        _currentSpeed = _data.NormalSpeed;
        _healthPresenter = new HPGaugePresenter(ServiceLocator.Get<GameUIManager>().HPGaugeView);
        _healthPresenter.Initialize(_data.Health);
        _gaugePresenter = new GaugePresenter(ServiceLocator.Get<GameUIManager>().GaugeView);
        _gaugePresenter.Initialize(_data.Gauge);
        _collider.GuardVisible(false);
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

            return;
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
        //少し遅らせてカメラのほうを向く
        var cam = Camera.main.transform.forward;
        cam.y = 0;
        cam.Normalize();
        transform.forward = Vector3.Slerp(transform.forward, cam, _rotateSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_gameManager.IsPaused) { return; }

        //AddForceなどはFixedUpdateで

        _rb.AddForce(Vector3.down * _data.FallSpeed, ForceMode.Acceleration);

        if (_isDashed)
        {
            _rb.MovePosition(_newPos);
        }
        if (_isJumped)
        {
            _rb.AddForce(Vector3.up * _data.FloatPower, ForceMode.Acceleration);
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
        _camForward = Camera.main.transform.forward;
        _camRight = Camera.main.transform.right;
        _camForward.y = _camRight.y = 0;
        _camForward.Normalize();
        _camRight.Normalize();

        //カメラの正面に合わせた移動方向
        _moveDir = _camForward * _velocity.y + _camRight * _velocity.x;

        //velocityに代入
        var vel = _moveDir * speed;
        var currentVel = _rb.linearVelocity;
        vel.y = currentVel.y;
        _rb.linearVelocity = vel;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (_gameManager.IsPaused) { return; }
        if (context.phase == InputActionPhase.Started)
        {
            if (!GaugeValueChange(-_data.JumpValue)) return;
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
        if (context.phase == InputActionPhase.Started && !_isDashed)
        {
            if (!GaugeValueChange(-_data.DashValue)) return;
            _isBoost = true;
            _data.DashTimer = 0;
            _dashStartPos = transform.position;
            Vector3 moveDir = _velocity != Vector2.zero ? _moveDir : _camForward;
            moveDir.Normalize();
            var rayCastDis = 8;
            _isDashed = true;
            if (Physics.Raycast(GetTargetCenter().position, moveDir, out RaycastHit hit, rayCastDis))
            {
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

        if (!_conflictObj)
        {
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

    public override void HitDamage(Collider other)
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
}
