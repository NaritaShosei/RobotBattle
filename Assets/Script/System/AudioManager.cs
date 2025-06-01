using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource _audio;
    void Start()
    {
        if (_audio == null)
        {
            _audio = GetComponent<AudioSource>();
        }
        ServiceLocator.Set(this);
    }

    public void PlayAudio(AudioData data)
    {
        _audio.PlayOneShot(data.AudioClip, data.Volume);
    }
}
