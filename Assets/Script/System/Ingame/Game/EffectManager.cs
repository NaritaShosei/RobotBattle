using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] Transform _parent;

    [SerializeField] ParticleSystem _explosion;

    Queue<ParticleSystem> _pool = new();
    List<ParticleSystem> _particles = new();
    public static EffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //プールに一定数保存
        for (int i = 0; i < 50; i++)
        {
            var effect = Instantiate(_explosion, _parent);
            effect.gameObject.SetActive(false);
            _pool.Enqueue(effect);
        }
        StartCoroutine(ReturnPool());
    }

    public void PlayExplosion(Vector3 pos)
    {
        //プールが空だったら生成
        if (_pool.Count == 0)
        {
            Debug.LogWarning("プールが空になりました");
            var newEffect = Instantiate(_explosion, _parent);
            newEffect.gameObject.SetActive(false);
            _pool.Enqueue(newEffect);
        }

        //エフェクトの生成
        var effect = _pool.Dequeue();
        effect.transform.position = pos;
        effect.gameObject.SetActive(true);
        _particles.Add(effect);
    }
    IEnumerator ReturnPool()
    {
        //プールに戻す処理
        //Updateでやると重いのでここでやる
        while (true)
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                if (_particles[i].gameObject.activeSelf == false)
                {
                    _pool.Enqueue(_particles[i]);
                    _particles.RemoveAt(i);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
