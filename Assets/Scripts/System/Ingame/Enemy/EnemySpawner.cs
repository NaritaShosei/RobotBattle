using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour, ISpawner
{
    [Header("生成データ")]
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private SpawnerData _data;

    [Header("ロックオンのターゲットになるTransform")]
    [SerializeField] private Transform _target;

    [Header("ドロップデータ")]
    [SerializeField] private EnemyDropData _dropData;

    public event Action<IEnemy> OnEnemySpawned;
    public event Action<ISpawner> OnDestroyed;

    private Camera _camera;

    private CancellationTokenSource _cts;


    private void Start()
    {
        _camera = Camera.main;
        _data.Health = _data.MaxHealth;
        _cts = new CancellationTokenSource();
    }

    public async UniTask Spawn(List<GameObject> enemies)
    {
        while (true)
        {
            float randInterval = Random.Range(_data.MinInterval, _data.MaxInterval);

            await UniTask.Delay((int)(randInterval * 1000), cancellationToken: _cts.Token);

            int randIndex = Random.Range(0, enemies.Count);

            var handle = InstantiateAsync(enemies[randIndex], _spawnTransform.position, Quaternion.identity);

            var obj = await handle;

            if (obj[0].TryGetComponent(out IEnemy enemy))
            {
                OnEnemySpawned?.Invoke(enemy);
            }
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IWeapon weapon))
            HitDamage(weapon);
    }

    public void HitDamage(IWeapon other)
    {
        _data.Health -= other.GetAttackPower();
        if (_data.Health <= 0)
        {
            // 死亡処理
            OnDestroyed?.Invoke(this);
            Dead();

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            Destroy(gameObject);
        }
    }

    private void Dead()
    {
        ServiceLocator.Get<ScoreManager>().AddScore(_dropData.Score);
        ServiceLocator.Get<MoneyManager>().AddMoney(_dropData.Money);
    }

}