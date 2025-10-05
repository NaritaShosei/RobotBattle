using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smooth = 1;
    Vector3 _velocity;
    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            _target.position,
            ref _velocity,
            _smooth);
    }
}
