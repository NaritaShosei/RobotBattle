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
    }
}