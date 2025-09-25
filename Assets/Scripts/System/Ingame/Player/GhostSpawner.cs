using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [Header("生成設定")]
    [SerializeField] private float _interval = 0.05f;
    [SerializeField] private float _lifeTime = 0.5f;
    [SerializeField] private Material _ghostMaterial;
    [SerializeField] private int _poolSize;

    private Queue<GameObject> _pool;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        // プールの生成
        _pool = new Queue<GameObject>(_poolSize);

        var parent = new GameObject($"Ghost_Pool_{gameObject.name}");

        for (int i = 0; i < _poolSize; i++)
        {
            // オブジェクトを生成
            var ghost = new GameObject($"Ghost_Pool_{i}");

            ghost.transform.SetParent(parent.transform);

            // メッシュの追加
            var mf = ghost.AddComponent<MeshFilter>();
            var mr = ghost.AddComponent<MeshRenderer>();

            mf.sharedMesh = new Mesh();

            mr.sharedMaterial = new Material(_ghostMaterial);

            ghost.SetActive(false);
            _pool.Enqueue(ghost);
        }
    }

    /// <summary>
    /// 分身の生成開始
    /// </summary>
    public async void StartSpawning()
    {
        _cts = new CancellationTokenSource();
        await SpawnLoop(_cts.Token);
    }

    private async UniTask SpawnLoop(CancellationToken token)
    {
        var smrArray = GetComponentsInChildren<SkinnedMeshRenderer>();

        if (smrArray == null || smrArray.Length == 0) { return; }

        while (!token.IsCancellationRequested)
        {
            try
            {
                // キャラだけでなく、装備なども複製
                foreach (var smr in smrArray)
                {
                    SpawnGhost(smr);
                }
                await UniTask.Delay((int)(1000 * _interval), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                // 正常終了
            }
        }
    }

    private void SpawnGhost(SkinnedMeshRenderer smr)
    {

        if (_pool.Count <= 0) { Debug.LogWarning("残像のプールが空"); return; }

        var ghost = _pool.Dequeue();
        ghost.SetActive(true);

        var mf = ghost.GetComponent<MeshFilter>();
        smr.BakeMesh(mf.sharedMesh);

        ghost.transform.SetPositionAndRotation(smr.transform.position, smr.transform.rotation);
        ghost.transform.localScale = smr.transform.lossyScale;
    }

    public void StopSpawning()
    {
        if (_cts == null) { return; }
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
}
