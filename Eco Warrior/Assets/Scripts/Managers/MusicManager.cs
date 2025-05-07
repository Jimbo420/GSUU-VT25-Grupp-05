using System;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Mixer Setup")]
    public AudioMixer audioMixer;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Music Clips")]
    public AudioClip calmMusic;
    public AudioClip tensionMusic;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlayCalmMusic()
    {
        musicSource.clip = calmMusic;
        musicSource.loop = true;
        musicSource.Play();
        audioMixer.FindSnapshot("Calm").TransitionTo(1f);
    }

    public void PlayTensionMusic()
    {
        musicSource.clip = tensionMusic;
        musicSource.loop = true;
        musicSource.Play();
        audioMixer.FindSnapshot("Tension").TransitionTo(1f);
    }
}


