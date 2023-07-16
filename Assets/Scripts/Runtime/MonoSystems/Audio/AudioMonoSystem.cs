using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Audio;

namespace PaperSouls.Runtime.MonoSystems.Audio
{
    internal sealed class AudioMonoSystem : MonoBehaviour, IAudioMonoSystem
    {
        [Header("Settings")]
        [Range(0f, 1f)] [SerializeField] private float _overallSound = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float _sfxSound = 1.0f;
        [Range(0f, 1f)] [SerializeField] private float _musicSound = 1.0f;

        [Header("Audio Clips/Sources")]
        [SerializeField] private List<AudioFile> _musicSounds;
        [SerializeField] private List<AudioFile> _sfxSounds;

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
        private void PlayMusic(string name)
        {
            AudioFile music = _musicSounds.Find(e => name.CompareTo(e.name) == 1);
            if (music == null) return;
            PlayMusic(music.audio, _overallSound * _musicSound);
        }

        /// <summary>   
        /// Finds and plays music with a given an id if such an Audio file exist.
        /// </summary>
        private void PlayMusic(int id)
        {
            AudioFile music = _musicSounds.Find(e => id == e.id);
            if (music == null) return;
            PlayMusic(music.audio, _overallSound * _musicSound);
        }

        /// <summary>   
        /// Finds and plays a SfX sound with a given name if such an Audio file exist.
        /// </summary>
        private void PlaySfX(string name, bool allowOverlay = true)
        {
            AudioFile sfx = _sfxSounds.Find(e => name.CompareTo(e.name) == 0);
            if (sfx == null) return;
            if (!(!allowOverlay && _sfxSource.isPlaying)) PlaySfX(sfx.audio, _overallSound * _sfxSound);
        }

        /// <summary>   
        /// Finds and plays a SfX sound with a given an id if such an Audio file exist.
        /// </summary>
        private void PlaySfX(int id, bool allowOverlay = true)
        {
            AudioFile sfx = _sfxSounds.Find(e => id == e.id);
            if (sfx == null) return;
            if (!allowOverlay && _sfxSource.isPlaying) PlaySfX(sfx.audio, _overallSound * _sfxSound);
        }

        /// <summary>
        /// Forces the SFX audio player to stop playing the current sound
        /// </summary>
        private void StopSfX()
        {
            _sfxSource.Stop();
        }

        private void StopMusic()
        {
            _musicSource.Stop();
        }

        private void PlayAudio(PlayAudioMessage msg)
        {
            switch (msg.AudioType)
            {
                case AudioType.Music:
                    if (msg.AudioName != null) PlayMusic(msg.AudioName);
                    else PlayMusic(msg.AudioID);
                    break;
                case AudioType.SfX:
                    if (msg.AudioName != null) PlaySfX(msg.AudioName, msg.AllowOverlay);
                    else PlaySfX(msg.AudioID, msg.AllowOverlay);
                    break;
                default:
                    Debug.LogWarning($"{msg.AudioType} is not a vaild Audio Type.");
                    break;
            }
        }

        private void StopAudio(StopAudioMessage msg)
        {
            switch (msg.AudioType)
            {
                case AudioType.Music:
                    StopMusic();
                    break;
                case AudioType.SfX:
                    StopSfX();
                    break;
                default:
                    Debug.LogWarning($"{msg.AudioType} is not a vaild Audio Type.");
                    break;
            }
        }

        private void AddListners()
        {
            GameManager.AddListener<PlayAudioMessage>(PlayAudio);
            GameManager.AddListener<StopAudioMessage>(StopAudio);
        }

        public void SetOverallVolume(float volume)
        {
            _overallSound = volume;
        }


        public void SetSfXVolume(float volume)
        {
            _sfxSound = volume;
        }


        public void SetMusicVolume(float volume)
        {
            _musicSound = volume;
        }

        private void Awake()
        {
            SetOverallVolume(1.0f);
            SetSfXVolume(1.0f);
            SetMusicVolume(1.0f);
            AddListners();
        }
    }
}
