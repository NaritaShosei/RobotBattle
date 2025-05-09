using System;
using UnityEngine;

public class DamageReactionCollider : MonoBehaviour
{
    public Action<Collider> OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError(other.name);
        OnTriggerEnterEvent?.Invoke(other);
    }
}
