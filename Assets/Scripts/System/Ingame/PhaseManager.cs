using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private PhaseDataBase _phaseDataBase;

    private PhaseContext _context;
    private CancellationTokenSource _cts;

    private void Start()
    {
        _context = new PhaseContext(
            timeLineManager: ServiceLocator.Get<TimeLineManager>(),
            bossManager: ServiceLocator.Get<BossManager>(),
            enemyManager: ServiceLocator.Get<EnemyManager>(),
            ingameManager: ServiceLocator.Get<IngameManager>());

        _cts = new CancellationTokenSource();

        WaitAllPhase(_cts.Token).Forget(ex =>
        {
            if (!(ex is OperationCanceledException))
            {
                Debug.LogError(ex);
            }
        });
    }

    private async UniTask WaitAllPhase(CancellationToken token)
    {
        foreach (var phase in _phaseDataBase.AllPhaseData)
        {
            Debug.Log($"{phase.PhaseName}==Start Run");

            await phase.Run(_context, token);
        }

        Debug.Log("End");
    }

    private void OnDestroy()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
}
// フェーズ間で共有したいデータなど
public class PhaseContext
{
    public TimeLineManager TimeLineManager { get; }
    public BossManager BossManager { get; }
    public EnemyManager EnemyManager { get; }
    public IngameManager IngameManager { get; }

    public PhaseContext(TimeLineManager timeLineManager, BossManager bossManager, EnemyManager enemyManager, IngameManager ingameManager)
    {
        TimeLineManager = timeLineManager;
        BossManager = bossManager;
        EnemyManager = enemyManager;
        IngameManager = ingameManager;
    }
}