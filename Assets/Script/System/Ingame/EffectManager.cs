using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] Transform _parent;

    [SerializeField] ParticleSystem _explosion;
    public static EffectManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }

    public void PlayExplosion(Vector3 pos)
    {
        Instantiate(_explosion, pos, Quaternion.identity, _parent);
    }
}
