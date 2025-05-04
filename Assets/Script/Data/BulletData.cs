using UnityEngine;


[CreateAssetMenu(menuName = "BulletData",fileName = "BulletData")]
public class BulletData : ScriptableObject
{
    public float AttackPower => _attackPower;
    [SerializeField]
    float _attackPower = 1f;

    public float BreakPoint => _breakPoint;
    [SerializeField]
    float _breakPoint;
}
