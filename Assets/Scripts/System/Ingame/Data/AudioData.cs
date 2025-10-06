using UnityEngine;

[CreateAssetMenu(menuName = "GameData/AudioData", fileName = "AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField]
    AudioClip _audioClip;
    public AudioClip AudioClip => _audioClip;
    [SerializeField]
    float _volume;
    public float Volume => _volume;
}
