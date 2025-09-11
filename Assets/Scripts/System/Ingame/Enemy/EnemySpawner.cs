using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform _spawnTransform;

    public void Spawn(GameObject enemy)
    {
        Instantiate(enemy, _spawnTransform.position, Quaternion.identity);
    }
}
