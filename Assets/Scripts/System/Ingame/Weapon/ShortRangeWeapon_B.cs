using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;

public class ShortRangeWeapon_B : WeaponBase, IWeapon
{
    [SerializeField, Tooltip("敵から離れる距離")] private float _weaponDistance = 3f;
    [SerializeField, Tooltip("攻撃の半径")] private float _attackRadius = 3f;

    private IEnemySource _currentTarget;
    // Playerの参照
    private ILockOnTarget _root;

    private Vector3 _targetPos;

    private bool _isTracking;

    private LockOn _lockOn;

    private int _count;
    public override int Count => _count;

    // いらないかもだが一旦残しておく
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
    public override bool RequiresPlayerMovement() => _count > 0;

    /// <summary>
    /// 攻撃対象
    /// </summary>
    /// <returns></returns>
    public override Transform GetTarget()
    {
        if (_currentTarget != null)
            return _currentTarget.GetTargetCenter();

        return null;
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

        var dir = _root.GetTargetCenter().forward * _attackRadius;
        var attackCenter = centerPos + dir;

        // 計算した場所で攻撃
        var colls = Physics.OverlapSphere(attackCenter, _attackRadius);

        // Player以外にダメージを与える
        foreach (var coll in colls)
        {
            if (coll.TryGetComponent<IEnemySource>(out var enemy))
            {
                enemy.HitDamage(this);

                ServiceLocator.Get<EffectManager>().PlayExplosion(enemy.GetTransform().position);
            }
        }

        _count--;
    }

    public override void SetAttack(bool value)
    {
        _isAttack = value;

        if (_isAttack)
            Attack();
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
        await UniTask.Delay((int)(_data.CoolTime * 1000));

        _count = _data.AttackCapacity;
        Debug.Log("リロード完了!");
    }

    public override Vector3 GetTargetPos()
    {
        return _targetPos;
    }

    public override bool IKEnable()
    {
        return false;
    }

    public float GetAttackPower()
    {
        return _data.AttackPower;
    }

    public override bool CanAttackAnimPlay()
    {
        // 攻撃可能であればアニメーション再生可能
        return _count > 0;
    }

    /// <summary>
    /// デバッグ用：攻撃範囲の可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && _currentTarget != null)
        {
            var targetTransform = GetTarget();
            if (targetTransform != null)
            {
                Vector3 targetPos = targetTransform.position;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(targetPos, 0.5f);

                // 攻撃範囲
                if (transform.root.TryGetComponent(out ILockOnTarget component))
                {
                    var centerPos = component.GetTargetCenter().position;
                    var forward = component.GetTargetCenter().forward;
                    var attackCenter = centerPos + forward * _attackRadius;

                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(attackCenter, _attackRadius);
                }
            }
        }
    }
}
