using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private BossEnemy _boss;
    [SerializeField] private Transform _spawn;

    public void SpawnBoss()
    {
        Instantiate(_spawn, _spawn.position, Quaternion.identity);
    }
}
