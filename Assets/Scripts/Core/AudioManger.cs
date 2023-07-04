using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime;

namespace PaperSouls.Core
{
    public class AudioManger : MonoBehaviour
    {
        private static AudioManger _instance;
        private static readonly object Padlock = new();

        public static AudioManger Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new();
                    }

                    return _instance;
                }
            }
        }

        [Header("Settings")]
        [Range(0f, 1f)] public float overallSound = 1.0f;
        [Range(0f, 1f)] public float sfxSound = 1.0f;
        [Range(0f, 1f)] public float musicSound = 1.0f;

        [Header("Audio Clips/Sources")]
        [SerializeField] private List<Audio> _musicSounds;
        [SerializeField] private List<Audio> _sfxSounds;

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        /// <summary>   
        /// Private member that plays music given an audioclip and sound level between 0 and 1. 
        /// Music is play to compleation and only one audio clip can be played at once.
        /// </summary>
        private void PlayMusic(AudioClip audio, float sound)
        {
            _musicSource.volume = sound;
            _musicSource.clip = audio;
            _musicSource.Play();
        }

        /// <summary>   
        /// Private member that plays SfX sound given an audioclip and sound level between 0 and 1.
        /// Multiple audio files can be played at once. 
        /// </summary>
        private void PlaySfX(AudioClip audio, float sound)
        {
            _sfxSource.volume = sound;
            _sfxSource.PlayOneShot(audio);
        }

        /// <summary>   
        /// Finds and plays music with a given name if such an Audio file exist.
        /// </summary>
        public void PlayMusic(string name)
        {
            Audio music = _musicSounds.Find(e => name.CompareTo(e.name) == 1);
            if (music == null) return;
            PlayMusic(music.audio, overallSound * musicSound);
        }

        /// <summary>   
        /// Finds and plays music with a given an id if such an Audio file exist.
        /// </summary>
        public void PlayMusic(int id)
        {
            Audio music = _musicSounds.Find(e => id == e.id);
            if (music == null) return;
            PlayMusic(music.audio, overallSound * musicSound);
        }

        /// <summary>   
        /// Finds and plays a SfX sound with a given name if such an Audio file exist.
        /// </summary>
        public void PlaySFX(string name, bool allowOverlay = true)
        {
            Audio sfx = _sfxSounds.Find(e => name.CompareTo(e.name) == 0);
            if (sfx == null) return;
            if (!(!allowOverlay && _sfxSource.isPlaying)) PlaySfX(sfx.audio, overallSound * sfxSound);
        }

        /// <summary>   
        /// Finds and plays a SfX sound with a given an id if such an Audio file exist.
        /// </summary>
        public void PlaySFX(int id, bool allowOverlay = true)
        {
            Audio sfx = _sfxSounds.Find(e => id == e.id);
            if (sfx == null) return;
            if (!allowOverlay && _sfxSource.isPlaying) PlaySfX(sfx.audio, overallSound * sfxSound);
        }

        public void StopSFX()
        {
            _sfxSource.Stop();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
    }
}
