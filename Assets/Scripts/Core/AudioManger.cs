using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    private static AudioManger instance; 
    public static AudioManger Instance
    {
        get
        {
            if (instance == null) Debug.Log("Audio Manger is null!!!");

            return instance;
        }

        private set { }
    }

    [Header("Settings")]
    [Range(0f, 1f)] public float overallSound = 1.0f;
    [Range(0f, 1f)] public float sfxSound = 1.0f;
    [Range(0f, 1f)] public float musicSound = 1.0f;

    [HeaderAttribute("Audio Clips/Sources")]
    public List<Audio> musicSounds;
    public List<Audio> sfxSounds;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public void PlayMusic(string name)
    {
        Audio music = musicSounds.Find(e => name.CompareTo(e.name) == 1);
        if (music == null) return;
        musicSource.volume = overallSound * musicSound;
        musicSource.clip = music.audio;
        musicSource.Play();
    }

    public void PlayMusic(int id)
    {
        Audio music = musicSounds.Find(e => id == e.id);
        if (music == null) return;
        musicSource.volume = overallSound * musicSound;
        musicSource.clip = music.audio;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Audio sfx = sfxSounds.Find(e => name.CompareTo(e.name) == 0);
        if (sfx == null) return;
        sfxSource.volume = overallSound * sfxSound;
        sfxSource.PlayOneShot(sfx.audio);
    }

    public void PlaySFX(int id)
    {
        Audio sfx = sfxSounds.Find(e => id == e.id);
        if (sfx == null) return;
        sfxSource.volume = overallSound * sfxSound;
        sfxSource.PlayOneShot(sfx.audio);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }
}
