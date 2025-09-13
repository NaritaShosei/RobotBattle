using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour, ISpawner
{
    [Header("生成データ")]
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private SpawnerData _data;


    [Header("ロックオンのターゲットになるTransform")]
    [SerializeField] private Transform _target;

    public event Action<IEnemy> OnEnemySpawned;
    public event Action<ISpawner> OnDestroyed;

    private Camera _camera;


    private void Start()
    {
        _camera = Camera.main;
        _data.Health = _data.MaxHealth;
    }

    public async UniTask Spawn(List<GameObject> enemies)
    {
        while (true)
        {
            float randInterval = Random.Range(_data.MinInterval, _data.MaxInterval);

            await UniTask.Delay((int)(randInterval * 1000), cancellationToken: destroyCancellationToken);

            int randIndex = Random.Range(0, enemies.Count);

            var handle = InstantiateAsync(enemies[randIndex], _spawnTransform.position, Quaternion.identity);

            var obj = await handle;

            if (obj[0].TryGetComponent(out IEnemy enemy))
            {
                OnEnemySpawned?.Invoke(enemy);
            }
        }
    }

    public void AddOnAttackEvent(Action<PlayerController> action)
    {

    }

    public void RemoveOnAttackEvent(Action<PlayerController> action)
    {

    }

    public bool IsTargetInView()
    {
        Vector3 viewportPosition = _camera.WorldToViewportPoint(GetTargetCenter().position);

        // 画面内にいるかチェック
        if (viewportPosition.z < 0) return false;

        if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1) return false;

        return true;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Transform GetTargetCenter()
    {
        return _target;
    }

    public void HitDamage(Collider other)
    {
        if (other.TryGetComponent(out Bullet_B component))
        {
            _data.Health -= component.AttackPower;
            if (_data.Health <= 0)
            {
                // 死亡処理
                OnDestroyed?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HitDamage(other);
    }
}

[CreateAssetMenu(menuName = "SpawnerData", fileName = "SpawnerData")]
public class SpawnerData : ScriptableObject
{
    [SerializeField] private float _maxHealth = 1000;
    public float MaxHealth { get => _maxHealth; }

    private float _health;
    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            OnHealthChanged?.Invoke(value);
        }
    }

    public event Action<float> OnHealthChanged;

    [SerializeField] private float _minInterval;
    [SerializeField] private float _maxInterval;
    public float MinInterval => _minInterval;
    public float MaxInterval => _maxInterval;
}