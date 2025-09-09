using Script.System.Ingame;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class BossEnemy : Enemy_B<EnemyData_B>
{
    Rigidbody _rb;

    [SerializeField] EnemyData_B _dataBase;
    [SerializeField] EnemyDodgeZone _dodgeZone;

    float _playerDistance;
    Vector3 _startPos;
    Vector3 _targetPos;
    bool _isDodged;
    bool _isJumping;
    bool _canJump = true;
    bool CanMove => _data.MinDistance <= _playerDistance;
    bool IsDash => _data.DashMinDistance <= _playerDistance;
    bool IsAttack => _data.AttackDistance >= _playerDistance;
    void Start()
    {
        OnStart();
        Initialize(_dataBase);
        _rb = GetComponent<Rigidbody>();
        _player = FindAnyObjectByType<PlayerController>();
        _dodgeZone.OnTriggerEnterEvent += Dodge;
    }

    private void Update()
    {
        if (_gameManager.IsPaused) { return; }
        GaugeValueChange(_data.RecoveryValue * Time.deltaTime);
        Debug.Log($"{name}:{_data.Gauge}");
        var dirToPlayer = _player.transform.position - transform.position;
        transform.forward = new Vector3(dirToPlayer.x, 0, dirToPlayer.z);

        _playerDistance = dirToPlayer.magnitude;


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
        if (_gameManager.IsPaused) { return; }
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
    void Move(Vector3 target)
    {
        Vector3 vec = target - transform.position;
        Vector3 dir = vec.normalized;

        var currentVel = _rb.linearVelocity;

        var speed = IsDash ? _data.BoostSpeed : _data.NormalSpeed;

        currentVel.x = dir.x * speed;
        currentVel.z = dir.z * speed;

        _rb.linearVelocity = currentVel;
    }

    void Dodge(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyBullet")) return;
        if (!other.TryGetComponent(out MonoBehaviour _)) return;

        if (!GaugeValueChange(-_data.DashValue)) return;
        _dodgeZone.Collider.enabled = false;
        _data.DodgeTimer = Time.time;
        _isDodged = true;
        _data.DashTimer = 0;
        _startPos = transform.position;
        //カメラの左側にいたら右に避ける、右側にいたら左に避ける
        Vector3 dir = _camera.WorldToViewportPoint(transform.position).x <= 0.5f ? _camera.transform.right : -_camera.transform.right;
        Vector3 candidateTarget = transform.position + dir * _data.DashDistance;

        Vector3 viewportPos = _camera.WorldToViewportPoint(candidateTarget);

        if (viewportPos.x < 0 || viewportPos.x > 1)
        {
            //マジックナンバー
            viewportPos.x = Mathf.Clamp(viewportPos.x, 0.1f, 0.9f);
            candidateTarget = _camera.ViewportToWorldPoint(viewportPos);
        }
        _targetPos = candidateTarget;
    }

    /// <summary>
    /// とりあえずのデバッグ用
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(_data.Health);
    }
}
