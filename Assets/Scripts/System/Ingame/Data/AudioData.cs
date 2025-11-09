using UnityEngine;

[CreateAssetMenu(menuName = "GameData/AudioData", fileName = "AudioData")]
public class AudioData : ScriptableObject
{
    [Header("Key Name")]
    [SerializeField] private string _name;
    public string Name => _name;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip _audioClip;
    public AudioClip AudioClip => _audioClip;

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float _volume = 1f;
    public float Volume => _volume;

    [Header("Spatial Settings")]
    [SerializeField, Tooltip("0 = 完全2D, 1 = 完全3D")]
    [Range(0f, 1f)] private float _spatialBlend = 0f;
    public float SpatialBlend => _spatialBlend;

    [SerializeField, Tooltip("この距離まで最大音量で聞こえる")]
    private float _minDistance = 1f;
    public float MinDistance => _minDistance;

    [SerializeField, Tooltip("この距離で音が聞こえなくなる")]
    private float _maxDistance = 500f;
    public float MaxDistance => _maxDistance;

    [SerializeField] private AudioRolloffMode _rolloffMode = AudioRolloffMode.Logarithmic;
    public AudioRolloffMode RolloffMode => _rolloffMode;

    [SerializeField, Tooltip("カスタムロールオフカーブ (RolloffModeがCustomの場合のみ使用)")]
    private AnimationCurve _customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    public AnimationCurve CustomRolloffCurve => _customRolloffCurve;

    [Header("Advanced 3D Settings")]
    [SerializeField, Range(0f, 5f), Tooltip("ドップラー効果の強さ (0 = 無効)")]
    private float _dopplerLevel = 1f;
    public float DopplerLevel => _dopplerLevel;

    [SerializeField, Range(0, 256), Tooltip("スピーカー間の拡散度 (3Dサウンドのステレオ感)")]
    private int _spread = 0;
    public int Spread => _spread;

    [Header("Loop Settings")]
    [SerializeField] private bool _loop = false;
    public bool Loop => _loop;

    [Header("Priority")]
    [SerializeField, Range(0, 256), Tooltip("優先度 (0 = 最高, 256 = 最低)")]
    private int _priority = 128;
    public int Priority => _priority;

    [Header("Pitch Settings")]
    [SerializeField, Range(-3f, 3f)] private float _pitch = 1f;
    public float Pitch => _pitch;

    [SerializeField] private bool _randomizePitch = false;
    public bool RandomizePitch => _randomizePitch;

    [SerializeField, Tooltip("ピッチのランダム範囲")]
    private Vector2 _pitchRange = new Vector2(0.95f, 1.05f);
    public Vector2 PitchRange => _pitchRange;

    /// <summary>
    /// ランダムピッチを取得
    /// </summary>
    public float GetRandomPitch()
    {
        return _randomizePitch
            ? Random.Range(_pitchRange.x, _pitchRange.y)
            : _pitch;
    }

    /// <summary>
    /// AudioSourceに設定を適用
    /// </summary>
    public void ApplyToAudioSource(AudioSource source)
    {
        if (source == null)
        {
            Debug.LogError("AudioSource is null");
            return;
        }

        source.clip = _audioClip;
        source.volume = _volume;
        source.pitch = GetRandomPitch();
        source.loop = _loop;
        source.priority = _priority;

        // 3D設定
        source.spatialBlend = _spatialBlend;
        source.minDistance = _minDistance;
        source.maxDistance = _maxDistance;
        source.rolloffMode = _rolloffMode;
        source.dopplerLevel = _dopplerLevel;
        source.spread = _spread;

        if (_rolloffMode == AudioRolloffMode.Custom)
        {
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, _customRolloffCurve);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // エディタでの値検証
        if (_minDistance < 0f) _minDistance = 0f;
        if (_maxDistance < _minDistance) _maxDistance = _minDistance;

        if (_pitchRange.x > _pitchRange.y)
        {
            float temp = _pitchRange.x;
            _pitchRange.x = _pitchRange.y;
            _pitchRange.y = temp;
        }
    }
#endif
}