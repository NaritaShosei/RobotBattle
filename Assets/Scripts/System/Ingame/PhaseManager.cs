using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private PhaseDataSequence _phaseDataBase;
    [SerializeField] private PlayerManager _playerManager;

    private PhaseContext _context;
    private CancellationTokenSource _cts;
    public event Action OnComplete;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    private void Start()
    {
        InitializeContext();

        if (_playerManager is null) { _playerManager = FindAnyObjectByType<PlayerManager>(); }
        _playerManager.OnDead += StopRun;

        _cts = new CancellationTokenSource();

        WaitAllPhase(_cts.Token).Forget(ex =>
        {
            if (!(ex is OperationCanceledException))
            {
                Debug.LogWarning($"フェーズの中断\n{ex}");
            }
        });
    }

    private void InitializeContext()
    {
        _context = new PhaseContext(
            timeLineManager: ServiceLocator.Get<TimeLineManager>(),
            bossManager: ServiceLocator.Get<BossManager>(),
            enemyManager: ServiceLocator.Get<EnemyManager>(),
            ingameManager: ServiceLocator.Get<IngameManager>());
    }

    private async UniTask WaitAllPhase(CancellationToken token)
    {
        try
        {
            await UniTask.WaitUntil(() => !_context.IngameManager.IsPaused);

            foreach (var phase in _phaseDataBase.AllPhaseData)
            {
                token.ThrowIfCancellationRequested();

                Debug.Log($"{phase.PhaseName}==Start Run");

                await phase.Run(_context, token);
            }
        }
        finally
        {
            OnComplete?.Invoke();
            Debug.Log("Phase All End");
        }
    }

    private void StopRun()
    {
        if (_cts is null) { return; }
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }

    private void OnDestroy()
    {
        StopRun();
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