using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPhase", menuName = "GameData/PhaseData/BossPhase")]
public class BossPhase : PhaseData_B
{
    public override async UniTask Run(PhaseContext context, CancellationToken token)
    {
        var bm = context.BossManager;

        bm.SpawnBoss();

        await UniTask.WaitUntil(() => !bm.IsBossAlive, PlayerLoopTiming.Update, token);
    }
}
