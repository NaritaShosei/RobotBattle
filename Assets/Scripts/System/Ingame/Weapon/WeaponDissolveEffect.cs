using UnityEngine;
using DG.Tweening;

public class WeaponDissolveEffect : MonoBehaviour
{
    [SerializeField] private Renderer[] _renderers;

    private MaterialPropertyBlock _mpb;
    private float _dissolveAmount;
    private static readonly int DissolveProp = Shader.PropertyToID("_DissolveAmount");

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        SetDissolve(1f); // 初期は非表示
    }

    /// <summary>
    /// 武器の出現
    /// </summary>
    public void Spawn(float time)
    {
        DOTween.To(() => _dissolveAmount, SetDissolve, 0f, time);
    }

    /// <summary>
    /// 武器の消滅
    /// </summary>
    public void Despawn(float time)
    {
        DOTween.To(() => _dissolveAmount, SetDissolve, 1f, time);
    }

    /// <summary>
    /// マテリアルに値を送る
    /// </summary>
    /// <param name="value"></param>
    private void SetDissolve(float value)
    {
        _dissolveAmount = value;
        _mpb.SetFloat(DissolveProp, value);
        foreach (var r in _renderers)
            if (r) r.SetPropertyBlock(_mpb);
    }
}
