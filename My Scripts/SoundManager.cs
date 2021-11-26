using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance =  this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }   
    }
    
    public AudioData audioData;
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;
    
    [Space(10f)]
    
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] AudioMixer effectsMixer;
    
    public void MuteMusicAudioMixer(bool mute)
    {
        if (mute)
        {
            musicMixer.SetFloat("MusicVolume", -80f);
        }
        else
        {
            musicMixer.SetFloat("MusicVolume", -13.8f);
        }
    }
    public void MuteEffectsAudioMixer(bool mute)
    {
        if (mute)
        {
            effectsMixer.SetFloat("EffectsVolume", -80f);
        }
        else
        {
            effectsMixer.SetFloat("EffectsVolume", -13.8f);
        }
    }

    public void PlayEffectsSound(AudioClip clip)
    {
        effectsSource.PlayOneShot(clip);
    }
    public void PlayMusicSound(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void PlayEffectsSoundDelay(AudioClip clip, float delay)
    {
        StartCoroutine(PlayEffectsSoundDelayRoutine(clip, delay));
    }
    IEnumerator PlayEffectsSoundDelayRoutine(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        effectsSource.PlayOneShot(clip);
    }

   

    
}
