using System;
using UnityEngine;

public class EnemyDodgeZone : MonoBehaviour
{
    [SerializeField]
    Collider _collider;
    public Collider Collider => _collider;

    bool _isDodge;
    public bool IsDodge => _isDodge;

    public Action<Collider> OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }
}
