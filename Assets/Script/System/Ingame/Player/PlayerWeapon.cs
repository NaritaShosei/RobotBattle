using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using RootMotion.FinalIK;
using static UnityEngine.EventSystems.EventTrigger;
public class PlayerWeapon : LongRangeAttack_B
{
    [SerializeField]
    Transform _player;

    [SerializeField]
    Sprite _weaponIcon;
    public Sprite Icon => _weaponIcon;

    public int Count => _count;

    [SerializeField]
    LockOn _lockOn;

    Camera _camera;

    bool _isAttack;
    bool _isReload;
    public bool IsAttack { get => _isAttack; set => _isAttack = value; }
    IEnemy _enemy;
    AimIK _aimIK;
    Vector3 _aimTargetPos;

    void Start()
    {
        Start_B();
        _camera = Camera.main;
        _aimIK = _player.GetComponent<AimIK>();
    }

    void Update()
    {

        if (_isReload) return;

        TargetSet();

        if (IsAttack)
        {
            Attack();
        }
    }
    void Attack()
    {
        if (_bulletPool.Count != 0 && _count != 0)
        {
            float rate = 1 / _data.AttackRate;
            if (Time.time > _time + rate)
            {
                _time = Time.time;
                var bullet = _bulletPool.Dequeue();
                bullet.gameObject.SetActive(true);
                bullet.SetPosition(_muzzle.position);

                _enemy = _lockOn.GetTarget();
                bullet.SetTarget(_enemy);
                _count--;
                if (_enemy != null)
                {
                    bullet.transform.forward = _aimTargetPos - _muzzle.position;
                }
            }
        }
    }

    void TargetSet()
    {
        _enemy = _lockOn.GetTarget();

        if (_enemy == null)
        {
            Vector2 crosshairPos = _lockOn.GetCrosshairPos();

            Ray ray = _camera.ScreenPointToRay(crosshairPos);
            float dis = 1000;

            if (Physics.Raycast(ray, out RaycastHit hit, dis))
            {
                float playerDis = Vector3.Distance(ray.origin, _player.position);
                if (hit.distance > playerDis)
                {
                    _aimTargetPos = hit.point;
                    return;
                }
            }
            Vector3 origin = ray.origin;
            Vector3 direction = ray.direction.normalized;

            Vector3 endPos = origin + direction * dis;


            _aimTargetPos = endPos;
            return;
        }
        _aimTargetPos = _enemy.GetTransform().position;
    }


    public Vector3 GetTargetPos()
    {
        return _aimTargetPos;
    }

    public void SetAttack(bool value)
    {
        IsAttack = value;
        if (value && _count <= 0)
        {
            Reload().Forget();
        }
    }
    public async UniTaskVoid Reload()
    {
        if (_isReload) return;
        if (_count == _data.BulletCount) return;
        _isReload = true;
        Debug.LogWarning("Reload" + _count);
        await UniTask.Delay((int)(_data.ReloadInterval * 1000));
        _count = _data.BulletCount;
        Debug.LogWarning("Reload To Complete" + _count);
        _isReload = false;
    }

    public void IKEnable(bool enable)
    {
        _aimIK.enabled = enable;
    }
}
