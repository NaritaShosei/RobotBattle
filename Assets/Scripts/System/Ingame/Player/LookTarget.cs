using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smooth = 1;
    private Vector3 _velocity;

    private void Start()
    {
        transform.position = _target.position;
    }

    private void LateUpdate()
    {
        // 目標位置へ向かう滑らかな補間
        transform.position = Vector3.SmoothDamp(
            transform.position,
            _target.position,
            ref _velocity,
            _smooth);
    }
}
