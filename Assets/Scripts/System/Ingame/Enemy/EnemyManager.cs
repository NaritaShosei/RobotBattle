using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemySpawner> _spawners = new();
    [SerializeField] private List<GameObject> _spawnEnemies = new();

    List<IEnemySource> _enemies = new();
    public List<IEnemySource> Enemies => _enemies;
    public bool IsEnemyAllDefeated => _enemies.Count == 0;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    private async void Start()
    {
        InitializeEnemies();
        try
        {
            await WaitUntilGameResumed();
            await StartSpawnersAsync();
        }
        catch { }
    }

    /// <summary>
    /// 既存の敵をリストに登録（Playerを除外）
    /// </summary>
    private void InitializeEnemies()
    {
        _enemies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IEnemySource>()
            .ToList();
    }

    /// <summary>
    /// ポーズ解除まで待機
    /// </summary>
    private async UniTask WaitUntilGameResumed()
    {
        await UniTask.WaitUntil(
            () => !ServiceLocator.Get<IngameManager>().IsPaused,
            cancellationToken: destroyCancellationToken
        );
    }

    /// <summary>
    /// 各Spawnerの初期化とSpawn開始
    /// </summary>
    private async UniTask StartSpawnersAsync()
    {
        List<UniTask> tasks = new();

        foreach (var spawner in _spawners)
        {
            SetupSpawner(spawner);
            tasks.Add(spawner.Spawn(_spawnEnemies));
        }

        await UniTask.WhenAll(tasks);
    }

    /// <summary>
    /// Spawnerのイベント登録
    /// </summary>
    private void SetupSpawner(EnemySpawner spawner)
    {
        spawner.OnEnemySpawned += (enemy) => Add(enemy);
        spawner.OnDestroyed += (enemySpawner) => Remove(enemySpawner);
    }

    /// <summary>
    /// Listから除外
    /// </summary>
    /// <param name="enemy">除外するEnemy</param>
    public void Remove(IEnemySource enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }

        if (IsEnemyAllDefeated)
        {
            // TODOBoss戦に遷移
        }
    }

    /// <summary>
    /// Listに追加
    /// </summary>
    /// <param name="enemy">追加するEnemy</param>
    public void Add(IEnemySource enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }
}
