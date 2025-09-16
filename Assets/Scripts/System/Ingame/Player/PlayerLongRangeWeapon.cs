using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;
public class PlayerLongRangeWeapon : LongRangeAttack_B
{

    LockOn _lockOn;

    Camera _camera;

    bool _isAttack;
    bool _isReload;
    public bool IsAttack { get => _isAttack; set => _isAttack = value; }
    ILockOnTarget _enemy;

    Vector3 _aimTargetPos;


    protected override void OnInitialize()
    {
        base.OnInitialize();
        Start_B();
        _camera = Camera.main;
        _lockOn = ServiceLocator.Get<LockOn>();
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
    public override void Attack()
    {
        if (_bulletManager.IsPoolCount(this) && _count != 0)
        {
            float rate = 1 / _data.AttackRate;
            if (Time.time > _time + rate)
            {
                _time = Time.time;
                var bullet = _bulletManager.GetBullet(this);
                bullet.gameObject.SetActive(true);
                bullet.SetPosition(_muzzle.position);

                _enemy = _lockOn.GetTarget();
                bullet.SetTarget(_enemy as IFightable);
                _count--;
                if (_enemy == null)
                {
                    bullet.transform.forward = _aimTargetPos - _muzzle.position;
                }
            }
        }
    }

    void TargetSet()
    {
        // ロックオン対象を取得
        _enemy = _lockOn.GetTarget();

        // ロックオン対象がいない場合
        if (_enemy == null)
        {
            // クロスヘアのスクリーン座標を取得
            Vector2 crosshairPos = _lockOn.GetCrosshairPos();

            // クロスヘア位置からレイを飛ばす
            Ray ray = _camera.ScreenPointToRay(crosshairPos);

            // レイの最大距離
            float dis = _data.Range;

            // レイがヒットした場合
            if (Physics.Raycast(ray, out RaycastHit hit, dis))
            {
                // プレイヤーからレイ発射点までの距離を算出
                float playerDis = Vector3.Distance(ray.origin, transform.position);

                // ヒットした位置がプレイヤーより前方にある場合のみ採用
                if (hit.distance > playerDis)
                {
                    // 照準のターゲット座標をヒット地点に設定
                    _aimTargetPos = hit.point;
                    return;
                }
            }

            // 何にも当たらなかった場合はレイの終点をターゲットにする
            Vector3 origin = ray.origin;
            Vector3 direction = ray.direction.normalized;
            Vector3 endPos = origin + direction * dis;

            _aimTargetPos = endPos;
            return;
        }

        // ロックオン対象がいる場合はその位置をターゲットにする
        _aimTargetPos = _enemy.GetTransform().position;
    }


    /// <summary>
    /// ロックオン中のtargetを取得する
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetTargetPos()
    {
        return _aimTargetPos;
    }
    public override void SetAttack(bool value)
    {
        IsAttack = value;
        if (value && _count <= 0 && enabled)
        {
            Reload();
        }
    }
    protected override async UniTask OnReload()
    {
        if (_isReload) return;
        if (_count == _data.AttackCapacity) return;
        _isReload = true;
        Debug.LogWarning("Reload" + _count);

        // 1000ミリ秒に変換
        await UniTask.Delay((int)(_data.CoolTime * 1000));
        _count = _data.AttackCapacity;
        Debug.LogWarning("Reload To Complete" + _count);
        _isReload = false;
    }
    public override void IKEnable(AimIK ik, bool enable)
    {
        ik.enabled = enable;
    }
}

