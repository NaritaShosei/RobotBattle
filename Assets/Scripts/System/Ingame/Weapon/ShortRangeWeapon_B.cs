using RootMotion.FinalIK;
using System.Threading.Tasks;
using UnityEngine;

public class ShortRangeWeapon_B : WeaponBase, IWeapon
{
    [SerializeField, Tooltip("敵から離れる距離")] private float _weaponDistance = 3f;

    private ILockOnTarget _currentTarget;
    private ILockOnTarget _root;

    private Vector3 _targetPos;

    private bool _isTracking;

    private LockOn _lockOn;

    private int _count;
    public override int Count => _count;

    private bool _isAttack;

    private void Start()
    {
        _count = _data.AttackCapacity;

        Start_B();
    }


    /// <summary>
    /// 派生先でStartで実行したい処理をここに
    /// </summary>
    protected virtual void Start_B()
    {
        _lockOn = ServiceLocator.Get<LockOn>();

        if (!transform.root.TryGetComponent(out _root))
        {
            Debug.Log("装備中のキャラクターがILockOnTargetを継承していません");
        }

    }

    private void Update()
    {
        Update_B();
    }

    /// <summary>
    /// 派生先でUpdateで実行したい処理をここに
    /// </summary>
    protected virtual void Update_B()
    {
        // ターゲットの更新
        UpdateTarget();

        if (!_isAttack) { return; }

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
    /// 敵との最適な距離を計算
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected virtual Vector3 CalculateTrackingPosition()
    {
        _currentTarget = _lockOn.GetTarget();

        if (_currentTarget == null) return Vector3.zero;

        Vector3 enemyPos = _currentTarget.GetTargetCenter().position;
        Vector3 playerPos = _root.GetTargetCenter().position;

        // 敵からプレイヤーへの方向ベクトル
        Vector3 dirToPlayer = (playerPos - enemyPos).normalized;

        // 敵から最適距離だけ離れた位置を計算
        Vector3 targetPos = enemyPos + dirToPlayer * _weaponDistance;

        _targetPos = _currentTarget.GetTargetCenter().position;

        return targetPos;
    }

    public override void Attack()
    {
        if (_count <= 0) { Reload(); return; }

        // Playerの中心を取得、攻撃の中心の計算
        var centerPos = _root.GetTargetCenter().position;
        var dir = _root.GetTargetCenter().forward * (_data.Range * 0.5f);
        var attackCenter = centerPos + dir;

        // 計算した場所で攻撃
        var colls = Physics.OverlapSphere(attackCenter, _data.Range * 0.5f);

        // Player以外にダメージを与える
        foreach (var coll in colls)
        {
            if (coll.TryGetComponent<IFightable>(out var enemy)
                && enemy.GetTransform().root != _root.GetTransform().root)
            {
                enemy.HitDamage(this);
                Debug.LogError("Attack Success");
            }
        }

        _count--;
    }

    public override void SetAttack(bool value)
    {
        _isAttack = value;
    }

    public override async void Reload()
    {
        // 少し待って攻撃回数を回復
        if (_count >= _data.AttackCapacity)
        {
            Debug.Log("既に弾薬は満タンです");
            return;
        }

        Debug.Log("リロード開始...");

        // リロード時間を設定（近接武器なので短めに）
        await Task.Delay((int)(_data.CoolTime * 1000));

        _count = _data.AttackCapacity;
        Debug.Log("リロード完了!");
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
    /// <summary>
    /// デバッグ用：攻撃範囲の可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && _currentTarget != null)
        {
            // 移動目標位置
            Vector3 targetPos = CalculateTrackingPosition();
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(targetPos, 0.5f);

            // 攻撃範囲
            if (transform.root.TryGetComponent(out ILockOnTarget component))
            {
                var centerPos = component.GetTargetCenter().position;
                var forward = component.GetTargetCenter().forward;
                var attackCenter = centerPos + forward * (_data.Range * 0.5f);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackCenter, _data.Range);
            }
        }
    }
}
