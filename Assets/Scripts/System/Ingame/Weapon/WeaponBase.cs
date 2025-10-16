using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private WeaponDissolveEffect _weaponDissolveEffect;

    protected WeaponData _data;

    public WeaponData Data => _data;

    public virtual void Initialize(WeaponData data)
    {
        _data = data;
        OnInitialize();
    }

    public void PlayDissolveEffect(bool active, float time)
    {
        if (active)
            _weaponDissolveEffect.Spawn(time);
        else
            _weaponDissolveEffect.Despawn(time);
    }

    protected virtual void OnInitialize() { }
    public abstract void Attack();
    public abstract void SetAttack(bool value);
    public abstract void Reload();
    public abstract int Count { get; }
    public abstract Vector3 GetTargetPos();
    public abstract bool  IKEnable();
    public virtual bool RequiresPlayerMovement() { return false; }
    public virtual Transform GetTarget() { return null; }
    public virtual bool IsTrackingActive() { return false; }
}