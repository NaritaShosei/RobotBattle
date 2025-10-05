using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GhostFade : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;

    private float _spawnTime;
    private float _lifeTime;
    private Action _onComplete;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    public void Initialize(float lifeTime, Action onComplete = null)
    {
        _lifeTime = lifeTime;
        _spawnTime = Time.time;

        _onComplete = onComplete;

        UpdateProperties();

        StartLoop();
    }

    private async void StartLoop()
    {
        try
        {
            while (true)
            {
                await UniTask.WaitUntil(() => !ServiceLocator.Get<IngameManager>().IsPaused, cancellationToken: destroyCancellationToken);

                float age = Time.time - _spawnTime;

                UpdateProperties();

                if (age > _lifeTime)
                {
                    gameObject.SetActive(false);
                    _onComplete?.Invoke();
                    break;
                }

                await UniTask.Yield(cancellationToken: destroyCancellationToken);
            }
        }
        catch { }
    }

    /// <summary>
    /// materialのシェーダーに情報を渡す
    /// </summary>
    private void UpdateProperties()
    {
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetFloat("_SpawnTime", _spawnTime);
        _mpb.SetFloat("_CurrentTime", Time.time);
        _mpb.SetFloat("_LifeTime", _lifeTime);

        _renderer.SetPropertyBlock(_mpb);
    }
}
