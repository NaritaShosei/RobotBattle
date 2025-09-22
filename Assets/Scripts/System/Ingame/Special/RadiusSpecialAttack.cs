using Cysharp.Threading.Tasks;
using UnityEngine;

public class RadiusSpecialAttack : SpecialAttackBase
{
    [SerializeField] private SphereCollider _collider;

    private void Awake()
    {
        if (_collider == null)
        {
            _collider = GetComponent<SphereCollider>();
        }

        _collider.radius = Data.Range;
        _collider.enabled = false;
        _collider.isTrigger = true;
    }

    [ContextMenu("範囲必殺技")]
    public override async UniTask SpecialAttack()
    {
        _collider.enabled = true;

        try
        {
            await UniTask.Delay((int)(1000 * Data.Duration), cancellationToken: destroyCancellationToken);
        }
        catch { }

        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IEnemySource enemy))
        {
            enemy.HitDamage(this);
        }
    }
}
