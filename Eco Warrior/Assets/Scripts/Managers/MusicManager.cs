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
    private AudioClip _currentClip;
    public AudioClip calmMusic;
    public AudioClip tensionMusic;
    private float tensionTimer;
    private float tensionDuration = 5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlayCalmMusic()
    {
        if (_currentClip == calmMusic) return;
        _currentClip = calmMusic;

        musicSource.clip = calmMusic;
        musicSource.loop = true;
        musicSource.Play();
        audioMixer.FindSnapshot("Calm").TransitionTo(1f);
    }


    public void PlayTensionMusic()
    {
        Debug.Log("Trying to play Tension Music");

        if (_currentClip == tensionMusic) return; 
        _currentClip = tensionMusic;

        musicSource.clip = tensionMusic;
        musicSource.loop = true;
        musicSource.Play();
        audioMixer.FindSnapshot("Tension").TransitionTo(1f);
    }

    private void Start()
    {
        PlayCalmMusic(); // Spelar lugn musik vid start
    }
    public void TriggerTensionOnGunfire()
    {
        tensionTimer = 0f;
        PlayTensionMusic();
    }
    private void Update()
    {
        if (_currentClip == tensionMusic)
        {
            tensionTimer += Time.deltaTime;
            if (tensionTimer >= tensionDuration)
            {
                PlayCalmMusic();
            }
        }
    }
}


