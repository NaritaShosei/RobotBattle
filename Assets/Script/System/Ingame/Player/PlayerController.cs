using Script.System.Ingame;
using SymphonyFrameWork.System;
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
    Rigidbody _rb;
    InputBuffer _input;
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

    /// <summary>
    /// Debug用
    /// </summary>
    [SerializeField] Text b;
    void Start()
    {
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _rb = GetComponent<Rigidbody>();
        Initialize(_dataBase);
        _currentSpeed = _data.NormalSpeed;
        AddAction();
        _healthPresenter = new HPGaugePresenter(GameUIManager.Instance.HPGaugeView);
        _healthPresenter.Initialize(_data.Health);
        _gaugePresenter = new GaugePresenter(GameUIManager.Instance.GaugeView);
        _gaugePresenter.Initialize(_data.Gauge);
        Start_B();
    }

    void Update()
    {
        //Debug用
        b.text = "health" + _data.Health.ToString();

        _rb.AddForce(Vector3.down * _data.FallSpeed, ForceMode.Acceleration);

        if (!_isDashed && !_isJumped)
        {
            GaugeValueChange(_data.RecoveryValue * Time.deltaTime);
        }
        if (_isGuard)
        {
            _collider.GuardVisible(true);
        }
        else if (!_isGuard)
        {
            _collider.GuardVisible(false);
        }
        if (_isJumped)
        {
            if (!GaugeValueChange(-_data.JumpValue * Time.deltaTime))
            {
                _isJumped = false;
            }
            else
            {
                _rb.AddForce(Vector3.up * _data.FloatPower, ForceMode.Acceleration);
            }
        }

        if (_isBoost)
        {
            if (_velocity != Vector2.zero)
            {
                if (!GaugeValueChange(-_data.DashValue * Time.deltaTime))
                {
                    _isBoost = false;
                }
            }
        }

        if (_isDashed)
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

        else if (!_isDashed)
        {
            Move(_isBoost ? _data.BoostSpeed : _data.NormalSpeed);
        }
        var cam = Camera.main.transform.forward;
        cam.y = 0;
        transform.forward = cam;
    }

    private void FixedUpdate()
    {

        if (_isDashed)
        {
            _rb.MovePosition(_newPos);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
        _camForward = Camera.main.transform.forward;
        _camRight = Camera.main.transform.right;
        _camForward.y = _camRight.y = 0;
        _camForward.Normalize();
        _camRight.Normalize();
        _moveDir = _camForward * _velocity.y + _camRight * _velocity.x * 1.5f;

        var vel = _moveDir * speed;
        var currentVel = _rb.linearVelocity;
        vel.y = currentVel.y;
        _rb.linearVelocity = vel;
    }

    void Jump(InputAction.CallbackContext context)
    {
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

    void Dash(InputAction.CallbackContext context)
    {
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

    void Guard(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (_data.Gauge >= _data.GuardMinValue)
            {
                _isGuard = true;
            }
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            _isGuard = false;
        }
    }
    void OnGuard(Collider other)
    {
        if (other.TryGetComponent(out Bullet_B bullet))
        {
            if (!GaugeValueChange(-bullet.GuardBreakValue))
            {
                _isGuard = false;
            }
        }
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
        _collider.OnTriggerEnterEvent += OnGuard;
        _input.MoveAction.performed += OnMoveInput;
        _input.MoveAction.canceled += OnMoveInput;
        _input.JumpAction.started += Jump;
        _input.JumpAction.canceled += Jump;
        _input.DashAction.started += Dash;
        _input.DashAction.canceled += Dash;
        _input.GuardAction.started += Guard;
        _input.GuardAction.canceled += Guard;
    }

    void RemoveAction()
    {
        _collider.OnTriggerEnterEvent -= OnGuard;
        _input.MoveAction.performed -= OnMoveInput;
        _input.MoveAction.canceled -= OnMoveInput;
        _input.JumpAction.started -= Jump;
        _input.JumpAction.canceled -= Jump;
        _input.DashAction.started -= Dash;
        _input.DashAction.canceled -= Dash;
        _input.GuardAction.started -= Guard;
        _input.GuardAction.canceled -= Guard;
    }
}
