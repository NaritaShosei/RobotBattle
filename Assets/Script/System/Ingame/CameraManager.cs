using SymphonyFrameWork.System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineOrbitalFollow _freeLookCamera;
    [SerializeField, Range(0.1f, 5)]
    float _cameraSensitivityX = 2;
    [SerializeField]
    bool _invertX;
    [SerializeField, Range(0.1f, 5)]
    float _cameraSensitivityY = 2;
    [SerializeField]
    bool _invertY;
    InputBuffer _input;
    Vector2 _velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = ServiceLocator.GetInstance<InputBuffer>();
        _input.LookAction.performed += Look;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Look(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Performed)
        {
            _velocity = input;
        }
        if (_velocity.magnitude > 1)
        {
            _velocity = _velocity.normalized;
        }
        _freeLookCamera.HorizontalAxis.Value += _velocity.x
            * (_invertX ? _cameraSensitivityX : -_cameraSensitivityX);

        _freeLookCamera.VerticalAxis.Value = Mathf.Clamp(
        _freeLookCamera.VerticalAxis.Value += _velocity.y
        * (_invertY ? _cameraSensitivityY : -_cameraSensitivityY),
        _freeLookCamera.VerticalAxis.Range.x,//Rangの範囲内に固定する
        _freeLookCamera.VerticalAxis.Range.y);
    }
    private void OnDisable()
    {
        _input.LookAction.performed -= Look;
    }
}
