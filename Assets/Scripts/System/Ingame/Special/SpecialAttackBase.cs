using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SpecialAttackBase : MonoBehaviour, IWeapon
{
    private SpecialData _data;
    public SpecialData Data => _data;

    public float GetAttackPower()
    {
        return Data.AttackPower;
    }

    public void Initialize(SpecialData data)
    {
        _data = data;
        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    /// <summary>
    /// 必殺技の発動
    /// </summary>
    public abstract UniTask SpecialAttack();

}
