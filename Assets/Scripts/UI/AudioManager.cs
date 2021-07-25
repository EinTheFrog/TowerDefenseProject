using UnityEngine;

namespace UI
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private float musicVolumeMultiplier = 1;
        
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
                audioSource.volume = audioSource.CompareTag(MusicTag) ? musicVolume * musicVolumeMultiplier : audioVolume / 4;
            }
        }
    }
}
