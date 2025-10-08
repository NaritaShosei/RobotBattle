using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPhase", menuName = "GameData/PhaseData/EnemyPhase")]
public class EnemyPhase : PhaseData_B
{
    public override async UniTask Run(PhaseContext context, CancellationToken token)
    {
        var em = context.EnemyManager;

        await em.WaitUntilGameResumed();

        em.StartSpawnersAsync().Forget(ex => Debug.LogError(ex.Message));

        await UniTask.WaitUntil(() => em.IsEnemyAllDefeated, PlayerLoopTiming.Update, token);
    }
}
