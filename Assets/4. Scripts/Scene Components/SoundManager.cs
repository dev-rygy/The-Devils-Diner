using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager main;

    private static string musicVolumeValueString = "MUSIC_VOLUME_VALUE";
    private static string sfxVolumeValueString = "SFX_VOLUME_VALUE";

    [Header("Settings")]
    [SerializeField]
    private AudioSource musicAudioSource;
    [SerializeField]
    private AudioSource sfxAudioSource;

    [Header("Debugs")]
    [SerializeField]
    private AudioClip testAudio;

    public float MusicVolume
    {
        get { return musicAudioSource.volume; }
        set 
        {
            value = Mathf.Clamp01(value);
            musicAudioSource.volume = value;
            PlayerPrefs.SetFloat(musicVolumeValueString, value);
        }
    }

    public float SfxVolume
    {
        get { return sfxAudioSource.volume; }
        set
        {
            value = Mathf.Clamp01(value);
            sfxAudioSource.volume = value;
            PlayerPrefs.SetFloat(sfxVolumeValueString, value);
        }
    }

    private void Awake()
    {
        if(main == null)
            main = this;
        else
            Destroy(gameObject);

        musicAudioSource.volume = PlayerPrefs.GetFloat(musicVolumeValueString);
        sfxAudioSource.volume = PlayerPrefs.GetFloat(sfxVolumeValueString);
    }

    [ContextMenu("Test")]
    public void Test()
    {
        if(testAudio)
            PlayOneShot(testAudio);
    }

    public void PlayOneShot(AudioClip clip)
    {
        sfxAudioSource.pitch = 1;
        sfxAudioSource.PlayOneShot(clip);
    }

    public void PlayOneShotRandom(AudioClip[] clips)
    {
        sfxAudioSource.pitch = 1;
        sfxAudioSource.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]);
    }
}
