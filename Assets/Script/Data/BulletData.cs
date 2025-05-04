using UnityEngine;


[CreateAssetMenu(menuName = "BulletData",fileName = "BulletData")]
public class BulletData : ScriptableObject
{
    [SerializeField]
    float _attackPower = 1f;
    public float AttackPower => _attackPower;

    [SerializeField]
    float _guardBreakValue;
    public float GuardBreakValue => _guardBreakValue;
    [SerializeField] 
    float _moveSpeed = 100;
    public float MoveSpeed => _moveSpeed;
    [SerializeField]
    float _minDistance = 20;
    public float MinDistance => _minDistance;
    [SerializeField]
    float _enableTime = 5;
    public float EnableTime => _enableTime;
}
