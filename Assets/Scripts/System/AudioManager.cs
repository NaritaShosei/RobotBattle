using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    [Header("Audio Data")]
    [SerializeField] private AudioData[] _bgmDatas;
    [SerializeField] private AudioData[] _seDatas;

    [Header("Audio Source Settings")]
    [SerializeField] private int _seAudioSourceCount = 30;
    [SerializeField, Range(0f, 1f)] private float _masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float _bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float _seVolume = 1f;

    [Header("Fade Settings")]
    [SerializeField] private float _bgmFadeDuration = 1f;

    // BGM用
    private AudioSource _bgmAudioSource;
    private Coroutine _bgmFadeCoroutine;
    private string _currentBGMKey;

    // SE用（プーリング）
    private Queue<AudioSource> _seAudioSourcePool;
    private List<ActiveAudioSource> _activeAudioSources;

    // 辞書
    private Dictionary<string, AudioData> _bgmDict;
    private Dictionary<string, AudioData> _seDict;

    // アクティブなAudioSourceの情報
    private class ActiveAudioSource
    {
        public AudioSource Source;
        public float StartTime;
        public float Duration;
        public Transform FollowTarget;
    }

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeDictionary();
        InitializeAudioSources();

        ServiceLocator.Set(this);
    }

    private void Update()
    {
        UpdateActiveAudioSources();
    }

    #endregion

    #region Initialization

    private void InitializeDictionary()
    {
        _bgmDict = new Dictionary<string, AudioData>(_bgmDatas.Length);
        _seDict = new Dictionary<string, AudioData>(_seDatas.Length);

        // BGMの登録
        foreach (var bgm in _bgmDatas)
        {
            if (bgm == null)
            {
                Debug.LogWarning("Null BGM data found in array.");
                continue;
            }

            if (string.IsNullOrEmpty(bgm.Name))
            {
                Debug.LogWarning($"BGM data has empty name: {bgm.name}");
                continue;
            }

            if (_bgmDict.ContainsKey(bgm.Name))
            {
                Debug.LogError($"Duplicate BGM key found: {bgm.Name}");
                continue;
            }

            _bgmDict.Add(bgm.Name, bgm);
        }

        // SEの登録
        foreach (var se in _seDatas)
        {
            if (se == null)
            {
                Debug.LogWarning("Null SE data found in array.");
                continue;
            }

            if (string.IsNullOrEmpty(se.Name))
            {
                Debug.LogWarning($"SE data has empty name: {se.name}");
                continue;
            }

            if (_seDict.ContainsKey(se.Name))
            {
                Debug.LogError($"Duplicate SE key found: {se.Name}");
                continue;
            }

            _seDict.Add(se.Name, se);
        }

        Debug.Log($"AudioManager initialized. BGM: {_bgmDict.Count}, SE: {_seDict.Count}");
    }

    private void InitializeAudioSources()
    {
        // BGM用AudioSource
        _bgmAudioSource = GetComponent<AudioSource>();
        _bgmAudioSource.loop = true;
        _bgmAudioSource.playOnAwake = false;

        // SEプールの初期化
        _seAudioSourcePool = new Queue<AudioSource>(_seAudioSourceCount);
        _activeAudioSources = new List<ActiveAudioSource>(_seAudioSourceCount);

        for (int i = 0; i < _seAudioSourceCount; i++)
        {
            GameObject sourceObj = new GameObject($"SE_AudioSource_{i:D2}");
            sourceObj.transform.SetParent(transform);
            AudioSource source = sourceObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;

            _seAudioSourcePool.Enqueue(source); 
        }
    }

    #endregion

    #region BGM Control

    /// <summary>
    /// BGMを再生（フェードなし）
    /// </summary>
    public void PlayBGM(string key)
    {
        PlayBGM(key, false);
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    public void PlayBGM(string key, bool useFade)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("BGM key is null or empty.");
            return;
        }

        if (!_bgmDict.TryGetValue(key, out AudioData bgmData))
        {
            Debug.LogWarning($"BGM with key '{key}' not found.");
            return;
        }

        if (bgmData.AudioClip == null)
        {
            Debug.LogError($"BGM '{key}' has null AudioClip.");
            return;
        }

        // 同じBGMが再生中なら何もしない
        if (_currentBGMKey == key && _bgmAudioSource.isPlaying)
        {
            return;
        }

        _currentBGMKey = key;

        if (_bgmFadeCoroutine != null)
        {
            StopCoroutine(_bgmFadeCoroutine);
        }

        if (useFade)
        {
            _bgmFadeCoroutine = StartCoroutine(FadeBGM(bgmData));
        }
        else
        {
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Stop();
            }
            bgmData.ApplyToAudioSource(_bgmAudioSource);
            _bgmAudioSource.volume = bgmData.Volume * _bgmVolume * _masterVolume;
            _bgmAudioSource.Play();
        }
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBGM(bool useFade = false)
    {
        if (_bgmFadeCoroutine != null)
        {
            StopCoroutine(_bgmFadeCoroutine);
        }

        if (useFade)
        {
            _bgmFadeCoroutine = StartCoroutine(FadeOutBGM());
        }
        else
        {
            _bgmAudioSource.Stop();
            _currentBGMKey = null;
        }
    }

    /// <summary>
    /// BGMのフェード処理
    /// </summary>
    private IEnumerator FadeBGM(AudioData newBgmData)
    {
        float startVolume = _bgmAudioSource.volume;
        float targetVolume = newBgmData.Volume * _bgmVolume * _masterVolume;
        float elapsed = 0f;

        // フェードアウト
        while (elapsed < _bgmFadeDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (_bgmFadeDuration * 0.5f);
            _bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        // BGM切り替え
        _bgmAudioSource.Stop();
        newBgmData.ApplyToAudioSource(_bgmAudioSource);
        _bgmAudioSource.volume = 0f;
        _bgmAudioSource.Play();

        // フェードイン
        elapsed = 0f;
        while (elapsed < _bgmFadeDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (_bgmFadeDuration * 0.5f);
            _bgmAudioSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        _bgmAudioSource.volume = targetVolume;
        _bgmFadeCoroutine = null;
    }

    /// <summary>
    /// BGMフェードアウト
    /// </summary>
    private IEnumerator FadeOutBGM()
    {
        float startVolume = _bgmAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < _bgmFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _bgmFadeDuration;
            _bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        _bgmAudioSource.Stop();
        _currentBGMKey = null;
        _bgmFadeCoroutine = null;
    }

    #endregion

    #region SE Control (2D)

    /// <summary>
    /// SEを再生（2D）
    /// </summary>
    public AudioSource PlaySE(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("SE key is null or empty.");
            return null;
        }

        if (!_seDict.TryGetValue(key, out AudioData seData))
        {
            Debug.LogWarning($"SE with key '{key}' not found.");
            return null;
        }

        if (seData.AudioClip == null)
        {
            Debug.LogError($"SE '{key}' has null AudioClip.");
            return null;
        }

        AudioSource source = GetAvailableAudioSource();
        if (source == null)
        {
            Debug.LogWarning("No available AudioSource for SE. All sources are in use.");
            return null;
        }

        seData.ApplyToAudioSource(source);
        source.volume = seData.Volume * _seVolume * _masterVolume;
        source.spatialBlend = 0f; // 強制的に2Dにする
        source.Play();

        RegisterActiveAudioSource(source, seData.AudioClip.length, null);
        return source;
    }

    #endregion

    #region SE Control (3D)

    /// <summary>
    /// SEを3D位置で再生
    /// </summary>
    public AudioSource PlaySE3D(string key, Vector3 position)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("SE key is null or empty.");
            return null;
        }

        if (!_seDict.TryGetValue(key, out AudioData seData))
        {
            Debug.LogWarning($"SE with key '{key}' not found.");
            return null;
        }

        if (seData.AudioClip == null)
        {
            Debug.LogError($"SE '{key}' has null AudioClip.");
            return null;
        }

        AudioSource source = GetAvailableAudioSource();
        if (source == null)
        {
            Debug.LogWarning("No available AudioSource for SE. All sources are in use.");
            return null;
        }

        source.transform.position = position;
        seData.ApplyToAudioSource(source);
        source.volume = seData.Volume * _seVolume * _masterVolume;
        source.Play();

        RegisterActiveAudioSource(source, seData.AudioClip.length, null);
        return source;
    }

    /// <summary>
    /// SEをTransformに追従させて再生
    /// </summary>
    public AudioSource PlaySE3D(string key, Transform followTarget)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("SE key is null or empty.");
            return null;
        }

        if (followTarget == null)
        {
            Debug.LogWarning("Follow target is null.");
            return null;
        }

        if (!_seDict.TryGetValue(key, out AudioData seData))
        {
            Debug.LogWarning($"SE with key '{key}' not found.");
            return null;
        }

        if (seData.AudioClip == null)
        {
            Debug.LogError($"SE '{key}' has null AudioClip.");
            return null;
        }

        AudioSource source = GetAvailableAudioSource();
        if (source == null)
        {
            Debug.LogWarning("No available AudioSource for SE. All sources are in use.");
            return null;
        }

        source.transform.position = followTarget.position;
        seData.ApplyToAudioSource(source);
        source.volume = seData.Volume * _seVolume * _masterVolume;
        source.Play();

        RegisterActiveAudioSource(source, seData.AudioClip.length, followTarget);
        return source;
    }

    /// <summary>
    /// SEを停止
    /// </summary>
    public void StopSE(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        ReturnAudioSourceToPool(source);
    }

    /// <summary>
    /// 全てのSEを停止
    /// </summary>
    public void StopAllSE()
    {
        for (int i = _activeAudioSources.Count - 1; i >= 0; i--)
        {
            ActiveAudioSource active = _activeAudioSources[i];
            if (active.Source != null)
            {
                active.Source.Stop();
                ReturnAudioSourceToPool(active.Source);
            }
        }
        _activeAudioSources.Clear();
    }

    #endregion

    #region Audio Source Pooling

    /// <summary>
    /// 利用可能なAudioSourceを取得
    /// </summary>
    private AudioSource GetAvailableAudioSource()
    {
        // プールから取得
        if (_seAudioSourcePool.Count > 0)
        {
            return _seAudioSourcePool.Dequeue();
        }

        // プールが空の場合、再生中のものを探す
        // 優先度が低く、再生時間が長いものを停止して再利用
        ActiveAudioSource oldest = null;
        float oldestTime = float.MinValue;

        foreach (var active in _activeAudioSources)
        {
            if (active.Source != null && !active.Source.isPlaying)
            {
                // 既に再生終了しているものを優先
                ReturnAudioSourceToPool(active.Source);
                _activeAudioSources.Remove(active);
                return active.Source;
            }

            float playTime = Time.time - active.StartTime;
            if (playTime > oldestTime)
            {
                oldestTime = playTime;
                oldest = active;
            }
        }

        // 最も古いものを停止して再利用
        if (oldest != null)
        {
            oldest.Source.Stop();
            ReturnAudioSourceToPool(oldest.Source);
            _activeAudioSources.Remove(oldest);
            return oldest.Source;
        }

        Debug.LogError("Failed to get available AudioSource.");
        return null;
    }

    /// <summary>
    /// アクティブなAudioSourceを登録
    /// </summary>
    private void RegisterActiveAudioSource(AudioSource source, float duration, Transform followTarget)
    {
        _activeAudioSources.Add(new ActiveAudioSource
        {
            Source = source,
            StartTime = Time.time,
            Duration = duration,
            FollowTarget = followTarget
        });
    }

    /// <summary>
    /// AudioSourceをプールに返却
    /// </summary>
    private void ReturnAudioSourceToPool(AudioSource source)
    {
        if (source == null) return;

        source.clip = null;
        source.loop = false;
        source.transform.SetParent(transform);
        source.transform.localPosition = Vector3.zero;
        _seAudioSourcePool.Enqueue(source);
    }

    /// <summary>
    /// アクティブなAudioSourceの更新
    /// </summary>
    private void UpdateActiveAudioSources()
    {
        for (int i = _activeAudioSources.Count - 1; i >= 0; i--)
        {
            ActiveAudioSource active = _activeAudioSources[i];

            if (active.Source == null)
            {
                _activeAudioSources.RemoveAt(i);
                continue;
            }

            // 追従処理
            if (active.FollowTarget != null)
            {
                active.Source.transform.position = active.FollowTarget.position;
            }

            // 再生終了チェック
            if (!active.Source.isPlaying)
            {
                ReturnAudioSourceToPool(active.Source);
                _activeAudioSources.RemoveAt(i);
            }
        }
    }

    #endregion

    #region Volume Control

    /// <summary>
    /// マスターボリュームを設定
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    /// <summary>
    /// BGMボリュームを設定
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
    }

    /// <summary>
    /// SEボリュームを設定
    /// </summary>
    public void SetSEVolume(float volume)
    {
        _seVolume = Mathf.Clamp01(volume);
        UpdateSEVolumes();
    }

    private void UpdateAllVolumes()
    {
        UpdateBGMVolume();
        UpdateSEVolumes();
    }

    private void UpdateBGMVolume()
    {
        if (_currentBGMKey != null && _bgmDict.TryGetValue(_currentBGMKey, out AudioData bgmData))
        {
            _bgmAudioSource.volume = bgmData.Volume * _bgmVolume * _masterVolume;
        }
    }

    private void UpdateSEVolumes()
    {
        foreach (var active in _activeAudioSources)
        {
            if (active.Source != null && active.Source.isPlaying)
            {
                // 元の音量比率を維持しながら更新
                // Note: AudioDataから元の音量を取得する方法がないため、
                // 厳密には完全な音量制御にはならない（改善の余地あり）
                active.Source.volume *= _seVolume * _masterVolume;
            }
        }
    }

    #endregion

    #region Utility

    /// <summary>
    /// BGMが再生中か
    /// </summary>
    public bool IsBGMPlaying()
    {
        return _bgmAudioSource.isPlaying;
    }

    /// <summary>
    /// 現在のBGMキーを取得
    /// </summary>
    public string GetCurrentBGMKey()
    {
        return _currentBGMKey;
    }

    /// <summary>
    /// 再生中のSE数を取得
    /// </summary>
    public int GetActiveAudioSourceCount()
    {
        return _activeAudioSources.Count;
    }

    #endregion
}