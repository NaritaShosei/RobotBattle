﻿
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineOrbitalFollow _freeLookCamera;
    [SerializeField, Range(0.1f, 10)]
    float _cameraSensitivityX = 2;
    [SerializeField]
    bool _invertX;
    [SerializeField, Range(0.1f, 5)]
    float _cameraSensitivityY = 2;
    [SerializeField]
    bool _invertY;
    InputManager _input;

    void OnEnable()
    {
        _input = ServiceLocator.Get<InputManager>();
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var input = _input.LookAction.ReadValue<Vector2>();

        if (input.sqrMagnitude > 1)
        {
            input = input.normalized;
        }
        _freeLookCamera.HorizontalAxis.Value += input.x
            * (_invertX ? _cameraSensitivityX : -_cameraSensitivityX);

        _freeLookCamera.VerticalAxis.Value = Mathf.Clamp(
        _freeLookCamera.VerticalAxis.Value += input.y
        * (_invertY ? _cameraSensitivityY : -_cameraSensitivityY),
        _freeLookCamera.VerticalAxis.Range.x,//Rangeの範囲内に固定する
        _freeLookCamera.VerticalAxis.Range.y);
    }
}
