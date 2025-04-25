using System;
using UnityEngine;

public class SmallEnemyTargetZone : MonoBehaviour
{
    public Action<Collider> OnTriggerEnterEvent;
    public Action<Collider> OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent.Invoke(other);
    }
}
