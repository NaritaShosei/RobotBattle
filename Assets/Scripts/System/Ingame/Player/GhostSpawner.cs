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
    private Queue<Mesh> _meshPool;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        // プールの生成
        _pool = new Queue<GameObject>(_poolSize);

        var length = GetComponentsInChildren<SkinnedMeshRenderer>().Length;

        var parent = new GameObject($"Ghost_Pool_{gameObject.name}");

        for (int i = 0; i < _poolSize; i++)
        {
            // オブジェクトを生成
            var ghost = new GameObject($"Ghost_Pool_{i}");

            ghost.transform.SetParent(parent.transform);

            // メッシュの追加
            var mf = ghost.AddComponent<MeshFilter>();
            var mr = ghost.AddComponent<MeshRenderer>();
            ghost.AddComponent<GhostFade>();

            mf.sharedMesh = new Mesh();

            mr.sharedMaterial = new Material(_ghostMaterial);

            ghost.SetActive(false);
            _pool.Enqueue(ghost);
        }

        // SkinnedMeshRendererの数倍の大きさで生成
        _meshPool = new Queue<Mesh>(_poolSize * length);

        // Mesh プールの事前生成
        for (int i = 0; i < _poolSize * length; i++)
        {
            _meshPool.Enqueue(new Mesh());
        }
    }

    /// <summary>
    /// 分身の生成開始
    /// </summary>
    public async void StartSpawning()
    {
        if (_cts != null) { return; }

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
                await UniTask.WaitUntil(() => !ServiceLocator.Get<IngameManager>().IsPaused, cancellationToken: token);

                SpawnGhost(smrArray);

                await UniTask.Delay((int)(1000 * _interval), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                // 正常終了
            }
        }
    }

    private void SpawnGhost(SkinnedMeshRenderer[] smrArray)
    {

        if (_pool.Count <= 0) { Debug.LogWarning("残像のプールが空"); return; }

        var ghost = _pool.Dequeue();
        ghost.SetActive(true);

        var mf = ghost.GetComponent<MeshFilter>();
        var mr = ghost.GetComponent<MeshRenderer>();

        // 座標を先に決める
        ghost.transform.SetPositionAndRotation(transform.position, transform.rotation);
        ghost.transform.localScale = transform.lossyScale;

        // 再利用している Mesh をクリアして書き換える
        Mesh combined = mf.sharedMesh;
        combined.Clear();

        // 複数SMRをまとめて1つのMeshにする
        CombineInstance[] combines = new CombineInstance[smrArray.Length];

        for (int i = 0; i < smrArray.Length; i++)
        {
            var smr = smrArray[i];

            var baked = _meshPool.Dequeue();
            smr.BakeMesh(baked);

            // ソース頂点（baked）は smr のローカル空間の頂点なので、
            // ghost ローカル空間へ変換する行列を用意する：
            //  ghost.worldToLocal * smr.localToWorld
            var mat = ghost.transform.worldToLocalMatrix * smr.transform.localToWorldMatrix;

            // 1つにしたいMesh
            combines[i].mesh = baked;
            // meshを合成先のローカル座標系に変換するための行列
            combines[i].transform = mat;
        }

        // 全てを合成（サブメッシュを1つにまとめ、行列を適用）
        combined.CombineMeshes(combines, mergeSubMeshes: true, useMatrices: true);

        // baked Mesh の破棄
        for (int i = 0; i < smrArray.Length; i++)
        {
            if (combines[i].mesh != null)
                ReturnMesh(combines[i].mesh);
        }


        ghost.GetComponent<GhostFade>().Initialize(_lifeTime, () =>
        {
            _pool.Enqueue(ghost);
        });
    }

    private void ReturnMesh(Mesh mesh)
    {
        mesh.Clear();
        _meshPool.Enqueue(mesh);
    }

    public void StopSpawning()
    {
        if (_cts == null) { return; }
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }

    private void OnDestroy()
    {
        if (_meshPool != null)
        {
            foreach (var mesh in _meshPool)
            {
                if (mesh != null)
                {
                    Destroy(mesh);
                }
            }
            _meshPool.Clear();
        }
    }

}
