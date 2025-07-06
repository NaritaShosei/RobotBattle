using UnityEngine;

[CreateAssetMenu(menuName = "AudioData", fileName = "AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField]
    AudioClip _audioClip;
    public AudioClip AudioClip => _audioClip;
    [SerializeField]
    float _volume;
    public float Volume => _volume;
}
