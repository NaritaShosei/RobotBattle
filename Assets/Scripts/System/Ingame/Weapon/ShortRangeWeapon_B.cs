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

    public override bool IsTrackingActive() => _isTracking;
    public override bool RequiresPlayerMovement() => _isTracking;

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
            var centerPos = component.GetTargetCenter().position;

            var dir = component.GetTargetCenter().forward * _data.Range;

            var colls = Physics.OverlapSphere(centerPos + dir, _data.Range);

            foreach (var coll in colls)
            {
                if (coll.TryGetComponent<IEnemy>(out var enemy))
                {
                    enemy.HitDamage(this);
                }
            }
        }
    }

    public override void SetAttack(bool value)
    {

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
