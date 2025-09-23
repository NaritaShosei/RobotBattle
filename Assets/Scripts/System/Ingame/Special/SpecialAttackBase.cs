using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SpecialAttackBase : MonoBehaviour, IWeapon
{
    // DebugようにSerializeFieldにしている。後で削除
    [SerializeField]
    private SpecialData _data;
    public SpecialData Data => _data;

    public float GetAttackPower()
    {
        return Data.AttackPower;
    }

    public void Initialize(SpecialData data)
    {
        _data = data;
    }

    /// <summary>
    /// 必殺技の発動
    /// </summary>
    public abstract UniTask SpecialAttack();

}
