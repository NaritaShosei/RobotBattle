using UnityEngine;

public class AttackData : ScriptableObject
{
    public float AttackRate { get => _attackRate; }
    [SerializeField]
    float _attackRate = 1f;
}
