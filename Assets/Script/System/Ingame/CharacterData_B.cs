using System;
using UnityEngine;

namespace Script.System.Ingame
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "GameData/CharacterData/CharacterData")]
    public class CharacterData_B : ScriptableObject
    {
        [SerializeField]
        private float _maxHealth;
        public float MaxHealth { get => _maxHealth; }

        [SerializeField]
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

        [SerializeField, Header("1秒間に回復するゲージ量")]
        private float _recoveryValue = 30;
        public float RecoveryValue => _recoveryValue;

        [SerializeField]
        private float _jumpValue;
        public float JumpValue => _jumpValue;

        [SerializeField]
        private float _dashValue;
        public float DashValue => _dashValue;

        public event Action<float> OnGaugeChanged;
    }
}