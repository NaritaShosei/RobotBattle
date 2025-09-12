using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemySpawner> _spawners = new();
    [SerializeField] private List<GameObject> _spawnEnemies = new();


    List<ILockOnTarget> _enemies = new();
    public List<ILockOnTarget> Enemies => _enemies;
    public bool IsEnemyAllDefeated => _enemies.Count == 0;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    private void Start()
    {
        //EnemyをすべてListに格納
        _enemies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ILockOnTarget>().ToList();

        // PlayerもILockOnTargetを継承してしまっているので直す必要がある
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].GetTransform().TryGetComponent(out PlayerController _))
            {
                _enemies.RemoveAt(i);
                break;
            }
        }

        foreach (var spawner in _spawners)
        {
            //       SpawnLoop(spawner);
        }
    }

    /// <summary>
    /// Listから除外
    /// </summary>
    /// <param name="enemy">除外するEnemy</param>
    public void Remove(ILockOnTarget enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
    }

    /// <summary>
    /// Listに追加
    /// </summary>
    /// <param name="enemy">追加するEnemy</param>
    public void Add(ILockOnTarget enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }
}
