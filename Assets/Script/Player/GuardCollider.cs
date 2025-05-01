using System;
using UnityEngine;

public class GuardCollider : MonoBehaviour
{
    [SerializeField]
    Collider _collider;
    [SerializeField]
    Renderer _renderer;

    public Action<Collider> OnTriggerEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    public void GuardVisible(bool visible)
    {
        _collider.enabled = visible;
        _renderer.enabled = visible;
    }
}
