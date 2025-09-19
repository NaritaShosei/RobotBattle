using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

public abstract class Bullet_B : MonoBehaviour, IWeapon
{
    [SerializeField] protected float _enableTime = 1.5f;
    protected ILockOnTarget _target;
    public System.Action ReturnPoolEvent;
    protected float _timer;

    protected bool _isTimeReturned;
    protected bool _isConflictReturned;
    public float GuardBreakValue => _weaponData.GuardBreakValue;
    public float AttackPower => _weaponData.AttackPower;

    protected WeaponData _weaponData;

    // 発射位置を記憶
    private Vector3 _startPos;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public virtual void Initialize(WeaponData data)
    {
        _weaponData = data;
    }

    private void OnEnable() => OnEnable_B();
    private void OnDisable() => OnDisable_B();

    protected virtual void OnEnable_B()
    {
        _isTimeReturned = false;
        _isConflictReturned = false;
        _startPos = transform.position;

        // キャンセルトークン更新
        _cts = new CancellationTokenSource();
        MoveAsync(_cts.Token).Forget();
    }
    protected virtual void OnDisable_B()
    {
        _cts?.Cancel();
        if (_isTimeReturned || _isConflictReturned)
        {
            ReturnPoolEvent?.Invoke();
        }
    }

    async UniTaskVoid MoveAsync(CancellationToken token)
    {
        while (!_isTimeReturned && !_isConflictReturned && !token.IsCancellationRequested)
        {
            float speed = _weaponData.AttackSpeed;
            transform.position += transform.forward * speed * Time.deltaTime;

            // 移動距離チェック
            if (Vector3.Distance(_startPos, transform.position) >= _weaponData.Range)
            {
                _isTimeReturned = true;
                gameObject.SetActive(false); // プールに返す
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    void Update()
    {
        OnUpdate();
    }

    abstract protected void OnUpdate();

    void OnTriggerEnter(Collider other)
    {
        if (ServiceLocator.Get<IngameManager>().IsGameEnd) { return; }
        if (other.CompareTag("IgnoreCollider")) { return; }
        Conflict(other);
    }

    protected abstract void Conflict(Collider other);

    public virtual void SetTarget(ILockOnTarget target)
    {
        _target = target;
        if (_target != null)
        {
            transform.forward = (target.GetTargetCenter().position - transform.position).normalized;
        }
    }

    public virtual void SetPosition(Vector3 pos)
    {
        _timer = 0;
        transform.position = pos;
        _startPos = pos;
    }

    public float GetAttackPower() => _weaponData.AttackPower;
}
