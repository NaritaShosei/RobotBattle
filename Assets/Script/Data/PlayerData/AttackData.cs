using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "GameData")]
public class AttackData : ScriptableObject
{
    public float AttackRate => _attackRate;
    [SerializeField]
    float _attackRate = 1f;
    public float AttackPower => _attackPower;
    [SerializeField]
    float _attackPower = 1f;
}
