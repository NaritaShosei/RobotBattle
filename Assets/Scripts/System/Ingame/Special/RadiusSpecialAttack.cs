using Cysharp.Threading.Tasks;
using UnityEngine;

public class RadiusSpecialAttack : SpecialAttackBase
{
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private GameObject _viewObj;

    private void Awake()
    {
        if (!_collider)
        {
            _collider = GetComponent<SphereCollider>();
        }

        _collider.radius = Data.Range;
        _collider.enabled = false;
        _collider.isTrigger = true;

        if (_viewObj)
        {
            _viewObj.transform.localScale = Vector3.one * _collider.radius * 2;
            _viewObj.SetActive(false);
        }
    }

    [ContextMenu("範囲必殺技")]
    public override async UniTask SpecialAttack()
    {
        _collider.enabled = true;

        if (_viewObj)
            _viewObj.SetActive(true);

        try
        {
            await UniTask.Delay((int)(1000 * Data.Duration), cancellationToken: destroyCancellationToken);
        }
        catch { }

        _collider.enabled = false;

        if (_viewObj)
            _viewObj.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IEnemySource enemy))
        {
            enemy.HitDamage(this);
        }
    }
}
