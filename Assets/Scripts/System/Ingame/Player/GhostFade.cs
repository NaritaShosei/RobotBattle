using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GhostFade : MonoBehaviour
{
    private Material _material;
    private float _lifeTime;
    private Action _onComplete;

    public void Initialize(MeshRenderer mr, float lifeTime, Action onComplete = null)
    {
        _material = new Material(mr.sharedMaterial);
        mr.material = _material;

        _lifeTime = lifeTime;
        _onComplete = onComplete;

        StartFade();
    }

    public async void StartFade()
    {
        try
        {
            await FadeLoop();
        }
        catch (OperationCanceledException)
        {
            // 正常終了
        }
    }

    private async UniTask FadeLoop()
    {
        float time = 0;

        // 色のプロパティの名前を取得
        string colorProp = _material.HasProperty("_BaseColor") ? "_BaseColor" : "_Color"; ;

        // プロパティから色を取得
        Color color = _material.GetColor(colorProp);

        while (_lifeTime > time)
        {
            // 線形補間でフェード
            if (!ServiceLocator.Get<IngameManager>().IsPaused)
            {
                float alpha = Mathf.Lerp(1, 0, time / _lifeTime);
                Color newColor = color; newColor.a = alpha;
                _material.SetColor(colorProp, newColor);

                time += Time.deltaTime;
            }

            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }

        // フェード完了時にActionを発火
        _onComplete?.Invoke();
        gameObject.SetActive(false);
    }
}
