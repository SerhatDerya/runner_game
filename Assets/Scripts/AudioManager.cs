using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip bakcground;
    public AudioClip pain;
    public AudioClip coin;
    public AudioClip swipe;
    public AudioClip jump;

    [Header("---------- Audio Settings ----------")]
    [SerializeField] private float musicVolume = 0.1f;
    [SerializeField] private float sfxVolume = 0.7f;

    private void Start()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        musicSource.clip = bakcground;
        musicSource.Play();
        musicSource.loop = true;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

}
