using System;
using UnityEngine;

public class EnemyDodgeZone : MonoBehaviour
{
    [SerializeField]
    Collider _collider;
    public Collider Collider => _collider;

    public Action<Collider> OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }
}
