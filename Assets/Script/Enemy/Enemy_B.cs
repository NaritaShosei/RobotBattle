using Script.System.Ingame;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_B : Character_B<EnemyData_B>
{
    Rigidbody _rb;
    PlayerController _player;
    [SerializeField] EnemyData_B _dataBase;
    [SerializeField] EnemyDodgeZone _dodgeZone;
    [SerializeField] Transform _bulletParent;
    [SerializeField] Transform _targetCenter;
    public Transform TargetCenter => _targetCenter;
    Vector3 _startPos;
    Vector3 _targetPos;
    bool _isDodged;
    bool _isJumping;
    bool _canJump = true;
    bool CanMove => _data.MinDistance <= (_player.transform.position - transform.position).magnitude;
    bool IsDash => _data.DashMinDistance <= (_player.transform.position - transform.position).magnitude;

    [SerializeField] Text a;
    void Start()
    {
        Initialize(_dataBase);
        _rb = GetComponent<Rigidbody>();
        _player = FindAnyObjectByType<PlayerController>();
        _dodgeZone.OnTriggerEnterEvent += Dodge;
    }

    private void Update()
    {
        //Debug用
        a.text = _data.Gauge.ToString();

        var dirToPlayer = _player.transform.position - transform.position;
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

        if (_canJump && !_isJumping && _player.transform.position.y > transform.position.y)
        {
            StartJump();
        }
        if (_isJumping)
        {
            _rb.AddForce(Vector3.up * _data.JumpPower, ForceMode.Acceleration); // Impulseより連続的に加速感ある

            if (_data.JumpTimer + _data.JumpDuration <= Time.time)
            {
                _isJumping = false;
            }
        }
        if (_data.JumpTimer + _data.JumpDuration + _data.JumpInterval <= Time.time)
        {
            _canJump = true;
        }

        if (_data.DodgeTimer + _data.DodgeInterval <= Time.time)
        {
            _dodgeZone.Collider.enabled = true;
        }
    }

    void Move(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.Normalize();
        transform.forward = new Vector3(dir.x, 0, dir.z);

        var currentVel = _rb.linearVelocity;

        var speed = IsDash ? _data.BoostSpeed : _data.NormalSpeed;

        currentVel.x = dir.x * speed;
        currentVel.z = dir.z * speed;

        _rb.linearVelocity = currentVel;
    }

    void Dodge(Collider other)
    {
        if (other.transform == _bulletParent || other.transform.IsChildOf(_bulletParent))
        {
            if (!GaugeValueChange(-_data.DashValue)) return;
            _dodgeZone.Collider.enabled = false;
            _data.DodgeTimer = Time.time;
            _isDodged = true;
            _data.DashTimer = 0;
            _startPos = transform.position;
            var dir = Random.Range(0, 2) == 0 ? transform.right : -transform.right;
            _targetPos = transform.position + dir * (_data.DashDistance);
        }
    }

    void StartJump()
    {
        if (!GaugeValueChange(-_data.JumpValue)) return;
        _isJumping = true;
        _canJump = false;
        _data.JumpTimer = Time.time;
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
