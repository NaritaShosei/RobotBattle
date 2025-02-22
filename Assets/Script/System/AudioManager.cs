using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;

    /// <summary>
    /// 各オーディオグループの種類
    /// </summary>
    public enum AudioType
    {
        Master,
        BGM,
        SoundEffect,
    }

    private Dictionary<AudioType, (AudioMixerGroup group, AudioSource source, float originalVolume)> _audioDict = new();

    [SerializeField] private List<AudioClip> _bgmList = new();
    private CancellationTokenSource _bgmChangeToken;

    private void Awake()
    {
        AudioSourceInit();

        _audioDict[AudioType.BGM].source.loop = true;
    }

    /// <summary>
    /// AudioDictの初期化
    /// </summary>
    private void AudioSourceInit()
    {
        foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
        {
            //Enumの名前を出す
            string name = type.ToString();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            //ミキサーグループを取得する
            AudioMixerGroup group = _mixer.FindMatchingGroups(name).FirstOrDefault();
            if (group)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = group;
                source.playOnAwake = false;

                //初期のボリュームを取得
                _mixer.GetFloat($"{name}_Volume", out float value);

                //各情報を追加
                _audioDict.Add(type, (group, source, value));
            }
            else
            {
                Debug.LogWarning($"{name} is not a valid AudioMixerGroup.");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void VolumeSliderChanged(AudioType type, float value)
    {
        if (value < 0 || 1 < value)
        {
            Debug.LogWarning("入力は無効な値です");
            return;
        }

        //デシベルで音量を割合変更
        float db = value * (_audioDict[type].originalVolume + 80) - 80;

        _mixer.SetFloat(type.ToString(), db);
    }

    /// <summary>
    /// ミキサーグループを出す
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public AudioMixerGroup GetMixerGroup(AudioType type) => _audioDict[type].group;

    /// <summary>
    /// BGMを変更する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="duration"></param>
    public async void BGMChanged(int index, float duration)
    {
        //前のBGM変更があれば止める
        if (_bgmChangeToken is { IsCancellationRequested: false })
        {
            _bgmChangeToken.Cancel();
        }

        //トークンを生成する
        _bgmChangeToken = new();
        var token = _bgmChangeToken.Token;
        
        AudioSource source = _audioDict[AudioType.BGM].source;
        AudioClip bgm = _bgmList[index];

        //BGMをフェードアウト
        try
        {
            while (source.volume > 0)
            {
                source.volume -= duration / 2 * Time.time;
                await Awaitable.NextFrameAsync(token);
            }
        }
        finally
        {
            source.volume = 0;

            //新たなクリップに差し替え
            source.Stop();
            source.clip = bgm;
            source.Play();
        }


        //BGMをフェードイン
        try
        {
            while (source.volume < 1)
            {
                source.volume += duration / 2 * Time.time;
                await Awaitable.NextFrameAsync(token);
            }
        }
        finally
        {
            source.volume = 1;
        }
    }
}