using Cysharp.Threading.Tasks;
using UnityEngine;

public class RadiusSpecialAttack : SpecialAttackBase
{
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private GameObject _viewObj;

    protected override void OnInitialize()
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

    public override async UniTask SpecialAttack()
    {
        try
        {
            _collider.enabled = true;

            if (_viewObj)
                _viewObj.SetActive(true);
            await UniTask.Delay((int)(1000 * Data.Duration), cancellationToken: destroyCancellationToken);
            _collider.enabled = false;

            if (_viewObj)
                _viewObj.SetActive(false);
        }
        catch { }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IEnemySource enemy))
        {
            enemy.HitDamage(this);
            ServiceLocator.Get<EffectManager>().PlayExplosion(enemy.GetTargetCenter().position);
        }
    }
}
