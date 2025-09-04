
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
    InGameManager _inGameManager;
    void Start()
    {
        _input = ServiceLocator.Get<InputManager>();
        _inGameManager = ServiceLocator.Get<InGameManager>();
    }

    void Update()
    {
        if (_inGameManager.IsGameEnd || _inGameManager.IsPaused) { return; }
       
        //inputの取得
        var input = _input.LookAction.ReadValue<Vector2>();

        if (input.sqrMagnitude > 1)
        {
            input = input.normalized;
        }

        //横移動
        _freeLookCamera.HorizontalAxis.Value += input.x
             //反転設定の適応
             * (_invertX ? _cameraSensitivityX : -_cameraSensitivityX);

        //縦移動
        _freeLookCamera.VerticalAxis.Value = Mathf.Clamp(
             _freeLookCamera.VerticalAxis.Value += input.y
             //反転設定の適応
             * (_invertY ? _cameraSensitivityY : -_cameraSensitivityY),
             _freeLookCamera.VerticalAxis.Range.x,//Rangeの範囲内に固定する
             _freeLookCamera.VerticalAxis.Range.y);
    }
}
