using System;
using UnityEngine;

namespace Script.System.Ingame
{
    [CreateAssetMenu(fileName = "SmallEnemyData", menuName = "GameData/CharacterData/SmallEnemyData")]
    public class CharacterData_B : ScriptableObject
    {
        [SerializeField]
        private float _maxHealth;
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


        [SerializeField]
        private float _maxGauge;
        public float MaxGauge { get => _maxGauge; }

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

        public event Action<float> OnGaugeChanged;
    }
}