using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<IEnemy> _enemies = new();
    public List<IEnemy> Enemies => _enemies;
    public bool IsEnemyAllDefeated => _enemies.Count == 0;
    void Start()
    {
        _enemies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IEnemy>().ToList();
    }

    void Update()
    {

    }
}
