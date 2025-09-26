
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow _freeLookCamera;
    [SerializeField] private CinemachineCamera _lookCamera;
    [SerializeField, Range(0.1f, 10)] private float _cameraSensitivityX = 2;

    [SerializeField] private bool _invertX;

    [SerializeField, Range(0.1f, 5)] private float _cameraSensitivityY = 2;

    [SerializeField] private bool _invertY;

    [SerializeField] private float _defaultLens = 56.3f;
    [SerializeField] private float _fastLens = 80f;
    [SerializeField, Range(0, 1)] private float _lerpValue = 0.8f;

    private float _currentLens;
    private bool _isFast;

    private InputManager _input;
    private IngameManager _inGameManager;

    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    void Start()
    {
        _input = ServiceLocator.Get<InputManager>();
        _inGameManager = ServiceLocator.Get<IngameManager>();
        _currentLens = _defaultLens;
    }

    void Update()
    {
        if (_inGameManager.IsGameEnd || _inGameManager.IsPaused) { return; }

        LerpLens();
        UpdateCameraLook();
    }

    private void UpdateCameraLook()
    {
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

    private void LerpLens()
    {
        if (_isFast)
        {
            _currentLens = Mathf.Lerp(_currentLens, _fastLens, _lerpValue);
            _lookCamera.Lens.FieldOfView = _currentLens;
        }

        else
        {
            _currentLens = Mathf.Lerp(_currentLens, _defaultLens, _lerpValue);
            _lookCamera.Lens.FieldOfView = _currentLens;
        }
    }
    public void SetFastMode(bool isFast)
    {
        _isFast = isFast;
    }
}
