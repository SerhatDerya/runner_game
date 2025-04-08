using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip background;
    public AudioClip pain;
    public AudioClip coin;
    public AudioClip swipe;
    public AudioClip jump;

    [Header("---------- Audio Settings ----------")]
    [SerializeField] private float musicVolume = 0.1f;
    [SerializeField] private float sfxVolume = 0.7f;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            musicSource.mute = PlayerPrefs.GetInt("MusicMute") == 1;
            sfxSource.mute = PlayerPrefs.GetInt("SFXMute") == 1;
        }
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        
        musicSource.clip = background;
        musicSource.Play();
        musicSource.loop = true;

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(musicSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { ChangeSFXVolume(sfxSlider.value); });
        
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void ChangeMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }
    public void ChangeSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
    public void MuteMusic()
    {
        musicSource.mute = !musicSource.mute;
        PlayerPrefs.SetInt("MusicMute", musicSource.mute ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void MuteSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        PlayerPrefs.SetInt("SFXMute", sfxSource.mute ? 1 : 0);
        PlayerPrefs.Save();
    }

}
