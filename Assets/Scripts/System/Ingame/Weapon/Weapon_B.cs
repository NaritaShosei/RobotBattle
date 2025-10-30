using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using System;
using UnityEngine;

public abstract class Weapon_B : MonoBehaviour
{
    [SerializeField] private WeaponDissolveEffect _weaponDissolveEffect;

    protected WeaponData _data;

    public WeaponData Data => _data;
    protected bool _isReloading;

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
    public abstract int Count { get; set; }
    public abstract Vector3 GetTargetPos();
    public abstract bool IKEnable();
    public virtual bool RequiresPlayerMovement() { return false; }
    public virtual Transform GetTarget() { return null; }
    public virtual bool IsTrackingActive() { return false; }
    public virtual bool CanAttackAnimPlay() { return true; }

    public event Action<float> OnReload;
    protected virtual void OnReloadInvoke(float duration) => OnReload?.Invoke(duration);

    public event Action<int> OnCountUpdate;
    protected virtual void OnCountUpdateInvoke(int count) => OnCountUpdate?.Invoke(count);
}