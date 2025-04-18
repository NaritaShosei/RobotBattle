using Script.System.Ingame;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

[RequireComponent(typeof(Rigidbody))]
public class Enemy_B : Character_B<CharacterData_B>
{
    Rigidbody _rb;
    PlayerController _player;
    [SerializeField] CharacterData_B _dataBase;
    [SerializeField] EnemyDodgeZone _dodgeZone;
    [SerializeField] Transform _bulletParent;
    Vector3 _startPos;
    Vector3 _targetPos;
    bool _isDodge;


    float _dodgeTimer;
    void Start()
    {
        Initialize(_dataBase);
        _rb = GetComponent<Rigidbody>();
        _player = FindAnyObjectByType<PlayerController>();
        _dodgeZone.OnTriggerEnterEvent += Dodge;
    }

    private void Update()
    {
        var dirToPlayer = _player.transform.position - transform.position;
        if (_isDodge)
        {
            _data.DashTimer += Time.deltaTime;

            var t = Mathf.Clamp01(_data.DashTimer / _data.DashTime);

            var newPos = Vector3.Lerp(_startPos, _targetPos, t);

            _rb.MovePosition(newPos);

            if (t >= 1)
            {
                _isDodge = false;
            }
        }
        else if (!_isDodge)
        {
            Move(_player.transform.position);
            if (_dodgeTimer + 5 <= Time.time)
            {
                _dodgeZone.Collider.enabled = true;
            }
        }
    }

    void Move(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.Normalize();
        dir.y = 0;
        transform.forward = dir;
        _rb.linearVelocity = dir * _data.NormalSpeed;
    }

    void Dodge(Collider other)
    {
        if (other.transform == _bulletParent || other.transform.IsChildOf(_bulletParent))
        {
            if (!GaugeValueChange(-_data.DashValue)) return;
            _dodgeZone.Collider.enabled = false;
            _dodgeTimer = Time.time;
            _isDodge = true;
            _data.DashTimer = 0;
            _startPos = transform.position;
            var dir = Random.Range(0, 2) == 0 ? transform.right : -transform.right;
            _targetPos = transform.position + dir * _data.DashDistance;
        }
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
