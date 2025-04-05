using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "GameData")]
public class AttackData : ScriptableObject
{
    public float AttackRate { get => _attackRate; }
    [SerializeField]
    float _attackRate = 1f;
}
