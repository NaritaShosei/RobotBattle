using RootMotion.FinalIK;
using System.Threading.Tasks;
using UnityEngine;

public class ShortRangeWeapon_B : WeaponBase, IWeapon
{
    [SerializeField, Tooltip("敵から離れる距離")] private float _weaponDistance = 3f;

    private ILockOnTarget _currentTarget;

    private Vector3 _targetPos;

    private bool _isTracking;

    private LockOn _lockOn;

    private int _count;
    public override int Count => _count;

    private bool _isAttack;

    private void Start()
    {
        _count = _data.AttackCapacity;
        _lockOn = ServiceLocator.Get<LockOn>();
        Start_B();
    }


    /// <summary>
    /// 派生先でStartで実行したい処理をここに
    /// </summary>
    protected virtual void Start_B() { }

    private void Update()
    {
        Update_B();
    }

    /// <summary>
    /// 派生先でUpdateで実行したい処理をここに
    /// </summary>
    protected virtual void Update_B()
    {
        if (!_isAttack) { return; }

        // ターゲットの更新
        UpdateTarget();
        Attack();
    }

    private void UpdateTarget()
    {
        _currentTarget = _lockOn.GetTarget();
        if (_currentTarget != null)
        {
            _targetPos = _currentTarget.GetTargetCenter().position;
            _isTracking = true;
        }
        else
        {
            _isTracking = false;
        }
    }


    public override bool IsTrackingActive() => _isTracking;

    // 近接武器なので常に true
    public override bool RequiresPlayerMovement() => true;

    /// <summary>
    /// 攻撃時に移動したい座標
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetDesiredPlayerPosition()
    {
        if (_currentTarget != null)
            return CalculateTrackingPosition();
        return Vector3.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected virtual Vector3 CalculateTrackingPosition()
    {
        _currentTarget = _lockOn.GetTarget();

        if (_currentTarget == null) return Vector3.zero;

        Vector3 enemyPos = _currentTarget.GetTargetCenter().position;
        Vector3 playerPos = transform.position;

        // 敵からプレイヤーへの方向ベクトル
        Vector3 dirToPlayer = (playerPos - enemyPos).normalized;

        // 敵から最適距離だけ離れた位置を計算
        Vector3 targetPos = enemyPos + dirToPlayer * _weaponDistance;

        _targetPos = _currentTarget.GetTargetCenter().position;

        return targetPos;
    }

    public override void Attack()
    {
        if (transform.root.TryGetComponent(out ILockOnTarget component))
        {
            // Playerの中心を取得、攻撃の中心の計算
            var centerPos = component.GetTargetCenter().position;
            var dir = component.GetTargetCenter().forward * (_data.Range * 0.5f);
            var attackCenter = centerPos + dir;

            // 計算した場所で攻撃
            var colls = Physics.OverlapSphere(attackCenter, _data.Range * 0.5f);

            // Player以外にダメージを与える
            foreach (var coll in colls)
            {
                if (coll.TryGetComponent<IFightable>(out var enemy)
                    && enemy.GetTransform().root != component.GetTransform().root)
                {
                    enemy.HitDamage(this);
                    Debug.LogError("Attack Cuccesu");
                }
            }
        }
    }

    public override void SetAttack(bool value)
    {
        _isAttack = value;
    }

    public override async void Reload()
    {
        // 少し待って攻撃回数を回復
        //  await 
    }

    public override Vector3 GetTargetPos()
    {
        return _targetPos;
    }

    public override void IKEnable(AimIK ik, bool enable)
    {
        // IKは不要
    }

    public float GetAttackPower()
    {
        return _data.AttackPower;
    }
}
