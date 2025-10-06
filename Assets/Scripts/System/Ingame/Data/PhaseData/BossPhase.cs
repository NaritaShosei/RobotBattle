using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPhase", menuName = "GameData/PhaseData/BossPhase")]
public class BossPhase : PhaseData_B
{
    public override async UniTask Run(PhaseContext context)
    {
        var bm = ServiceLocator.Get<BossManager>();

        bm.SpawnBoss();

        await UniTask.WaitUntil(() => !bm.IsBossAlive);
    }
}
