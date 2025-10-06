using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPhase", menuName = "GameData/PhaseData/EnemyPhase")]
public class EnemyPhase : PhaseData_B
{
    public override async UniTask Run(PhaseContext context)
    {
        var em = ServiceLocator.Get<EnemyManager>();

        await em.WaitUntilGameResumed();

        em.StartSpawnersAsync().Forget();

        await UniTask.WaitUntil(() => em.IsEnemyAllDefeated);
    }
}
