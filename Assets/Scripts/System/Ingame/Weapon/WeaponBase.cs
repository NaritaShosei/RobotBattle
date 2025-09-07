using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected WeaponData _data;

    public WeaponData Data => _data;

    public virtual void Initialize(WeaponData data)
    {
        _data = data;
        OnInitialize();
    }

    protected virtual void OnInitialize() { }
    public abstract void Attack();
    public abstract void SetAttack(bool value);
    public abstract void Reload();
    public abstract int Count { get; }
    public abstract Vector3 GetTargetPos();
    public abstract void IKEnable(AimIK ik, bool enable);
}