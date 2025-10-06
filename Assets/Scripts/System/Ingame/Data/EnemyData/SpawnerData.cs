using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/SpawnerData", fileName = "SpawnerData")]
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
