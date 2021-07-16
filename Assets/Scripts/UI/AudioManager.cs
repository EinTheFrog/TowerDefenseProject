using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource[] _audioSources = default;
    private const string MusicTag = "MusicSource";

    public void UpdateVolume(float audioVolume, float musicVolume)
    {
        if (_audioSources == null)
        {
            _audioSources = FindObjectsOfType<AudioSource>();
        }
        foreach (var audioSource in _audioSources)
        {
            audioSource.volume = audioSource.CompareTag(MusicTag) ? musicVolume : audioVolume;
        }
    }
}
