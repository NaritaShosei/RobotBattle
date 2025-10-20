using UnityEngine;
using DG.Tweening;

public class WeaponDissolveEffect : MonoBehaviour
{
    [SerializeField] private Renderer[] _renderers;

    [SerializeField] private Vector2 _clamp = new Vector2(-0.2f, 1.2f);

    private MaterialPropertyBlock _mpb;
    private float _dissolveAmount;
    private static readonly int DissolveProp = Shader.PropertyToID("_DissolveAmount");

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        SetDissolve(0); // 初期は非表示
    }

    /// <summary>
    /// 武器の出現
    /// </summary>
    public void Spawn(float time)
    {
        DOTween.To(() => _dissolveAmount, SetDissolve, _clamp.x, time);
    }

    /// <summary>
    /// 武器の消滅
    /// </summary>
    public void Despawn(float time)
    {
        DOTween.To(() => _dissolveAmount, SetDissolve, _clamp.y, time);
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
