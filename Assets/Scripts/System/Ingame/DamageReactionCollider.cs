using System;
using UnityEngine;

public class DamageReactionCollider : MonoBehaviour
{
    public event Action<IWeapon> OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IWeapon weapon))
            OnTriggerEnterEvent?.Invoke(weapon);
    }
}
