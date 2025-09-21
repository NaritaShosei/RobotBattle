using Cysharp.Threading.Tasks;
using UnityEngine;

public class RadiusSpecialAttack : SpecialAttackBase
{
    [ContextMenu("範囲必殺技")]
    public override async UniTask SpecialAttack()
    {
        float timer = 0;
        try
        {
            while (timer < Data.Duration)
            {
                // この必殺技は一旦Playerの中心に存在すること前提として作る
                var colliders = Physics.OverlapSphere(transform.position, Data.Range * 0.5f);

                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent(out IEnemySource enemy))
                    {
                        enemy.HitDamage(this);
                        Debug.LogError(enemy.GetTargetCenter().gameObject.name);
                    }
                }

                timer += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: destroyCancellationToken);
            }
        }
        catch { }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Data.Range * 0.5f);
        }
    }
}
