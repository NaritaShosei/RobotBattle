using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingEnemy : Enemy_B<EnemyData_B>
{
    private Rigidbody _rb;

    [SerializeField] private EnemyData_B _dataBase;
    [SerializeField] private EnemyDodgeZone _dodgeZone;
    [SerializeField] private EnemyDodgeClampData _dodgeClampData;

    [Tooltip("回避行動を無視したいレイヤー")]
    [SerializeField] private string[] _ignoreLayers;

    [System.Serializable]
    public struct EnemyDodgeClampData
    {
        public float Min;
        public float Max;
    }

    [Header("分離設定")]
    [SerializeField] private float _radius = 2;
    [SerializeField] private float _strength = 3;
    [SerializeField] private string _layerName = "Enemy";

    [Header("アニメーション設定")]
    [SerializeField] protected AnimationController _animController;
    [SerializeField] protected string _moveAnimationParamName = "MoveValue";

    private float PlayerDistance => Vector3.Distance(transform.position, _player.transform.position);

    private Vector3 _startPos;
    private Vector3 _targetPos;
    protected Vector3 _separationDirection;

    private bool _isDodged;
    private bool _isJumping;
    private bool _canJump = true;
    private bool CanMove => _data.MinDistance <= PlayerDistance;
    private bool IsDash => _data.DashMinDistance <= PlayerDistance;
    private bool IsAttack => _data.AttackDistance >= PlayerDistance;

    private async void Start()
    {
        OnStart();
        Initialize(_dataBase);
        _rb = GetComponent<Rigidbody>();

        // TODO:managerからPlayerの情報を渡される形にしたほうがいい
        _player = FindAnyObjectByType<PlayerController>();
        _dodgeZone.OnTriggerEnterEvent += SetDodgeTargetPosition;

        try
        {
            await CalculateSeparation();
        }
        catch { }
    }

    private void Update()
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }
        GaugeValueChange(_data.RecoveryValue * Time.deltaTime);

        SetAnimationParam();

        HandleRotation();

        if (IsAttack)
        {
            OnAttackEvent?.Invoke(_player);
        }

        if (_isDodged)
        {
            _data.DashTimer += Time.deltaTime;

            var t = Mathf.Clamp01(_data.DashTimer / _data.DashTime);

            var newPos = Vector3.Lerp(_startPos, _targetPos, t);

            _rb.MovePosition(newPos);

            if (t >= 1)
            {
                _isDodged = false;
            }
        }

        if (CanMove)
        {
            Move(_player.transform.position);
        }

        if (_data.DodgeTimer + _data.DodgeInterval <= Time.time)
        {
            _dodgeZone.Collider.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (_gameManager.IsPaused || _gameManager.IsInEvent) { return; }
        _rb.AddForce(Vector3.down * _data.FallSpeed, ForceMode.Force);

        if (_canJump && !_isJumping && _player.transform.position.y > transform.position.y)
        {
            _isJumping = true;
            _canJump = false;
        }

        if (_isJumping)
        {
            if (_player.transform.position.y > transform.position.y)
            {
                _rb.AddForce(Vector3.up * _data.FloatPower, ForceMode.Force); // Impulseより連続的に加速感ある

                if (!GaugeValueChange(-_data.JumpValue * Time.fixedDeltaTime))
                {
                    _isJumping = false;
                    _data.JumpTimer = Time.time;
                }
            }
            else
            {
                _isJumping = false;
            }
        }
        if (!_canJump && (Time.time >= _data.JumpInterval + _data.JumpTimer))
        {
            _canJump = true;
        }
    }

    protected virtual void HandleRotation()
    {
        if (_player != null)
        {
            Vector3 flatDir = _player.transform.position - transform.position;
            flatDir.y = 0f;

            if (flatDir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(flatDir);
            }
        }
    }

    protected virtual void Move(Vector3 target)
    {
        Vector3 vec = target - transform.position;
        Vector3 dir = vec.normalized;

        dir += _separationDirection;
        dir = dir.normalized;

        var currentVel = _rb.linearVelocity;

        var speed = IsDash ? _data.BoostSpeed : _data.NormalSpeed;

        currentVel.x = dir.x * speed;
        currentVel.z = dir.z * speed;

        _rb.linearVelocity = currentVel;
    }

    protected virtual async UniTask CalculateSeparation()
    {
        while (true)
        {
            // 分離方向のリセット
            var _separationDir = Vector3.zero;

            Collider[] colliders = Physics.OverlapSphere(GetTargetCenter().position, _radius);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject == gameObject) { continue; }
                if (collider.TryGetComponent(out IEnemySource enemy))
                {
                    Vector3 away = GetTargetCenter().position - enemy.GetTargetCenter().position;
                    float dis = away.magnitude;

                    // 0除算の回避
                    if (dis > 0)
                    {
                        _separationDir += away / dis;
                    }
                }
            }

            _separationDirection = _separationDir.normalized * _strength;

            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }
    }

    protected virtual void SetDodgeTargetPosition(Collider other)
    {
        int layerMask = LayerMask.GetMask(_ignoreLayers);

        if (((1 << other.gameObject.layer) & layerMask) != 0) return;

        if (!other.TryGetComponent(out MonoBehaviour _)) return;

        if (!GaugeValueChange(-_data.DashValue)) return;

        _dodgeZone.Collider.enabled = false;

        _data.DodgeTimer = Time.time;

        _isDodged = true;

        _data.DashTimer = 0;

        _startPos = transform.position;

        // カメラの左側にいたら右に避ける、右側にいたら左に避ける
        Vector3 dir = _camera.WorldToViewportPoint(transform.position).x <= 0.5f ?
            _camera.transform.right : -_camera.transform.right;

        // 現在位置から回避方向に設定された距離分移動した候補位置を計算
        Vector3 candidateTarget = transform.position + dir * _data.DashDistance;

        // 候補位置をビューポート座標に変換して画面内におさまるかチェック
        Vector3 viewportPos = _camera.WorldToViewportPoint(candidateTarget);

        if (viewportPos.x < 0 || viewportPos.x > 1)
        {
            // ビューポートのX座標を設定された範囲内にクランプ
            viewportPos.x = Mathf.Clamp(viewportPos.x, _dodgeClampData.Min, _dodgeClampData.Max);

            // 補正されたビューポート座標をワールド座標に戻す
            candidateTarget = _camera.ViewportToWorldPoint(viewportPos);
        }

        // 最終的な目標位置を設定
        _targetPos = candidateTarget;
    }

    protected virtual void SetAnimationParam()
    {
        _animController.SetFloat(_moveAnimationParamName, _rb.linearVelocity.magnitude);
    }
}
