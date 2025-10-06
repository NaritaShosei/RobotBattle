using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private BossEnemy _boss;
    [SerializeField] private Transform _spawn;
    private BossEnemy _enemy;
    public bool IsBossAlive => _enemy.IsAlive;
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    public void SpawnBoss()
    {
        _enemy = Instantiate(_boss, _spawn.position, Quaternion.identity).GetComponent<BossEnemy>();
        ServiceLocator.Get<EnemyManager>().Add(_enemy);
    }
}
