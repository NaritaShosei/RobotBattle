using Script.System.Ingame;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "GameData/CharacterData/PlayerData")]

public class PlayerData : CharacterData_B
{
    [SerializeField]
    private float _maxGauge;
    public float MaxGauge { get => _maxGauge; }

    [SerializeField]
    private float _gauge;
    public float Gauge
    {
        get => _gauge;
        set
        {
            _gauge = value;
            OnGaugeChanged?.Invoke(value);
        }
    }
    [SerializeField]
    private float _jumpValue;
    public float JumpValue => _jumpValue;

    [SerializeField]
    private float _dashValue;
    public float DashValue => _dashValue;

    public event Action<float> OnGaugeChanged;
}
