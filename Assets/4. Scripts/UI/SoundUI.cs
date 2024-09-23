using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundUI : MonoBehaviour
{
    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sfxSlider;

    public void Initialize()
    {
        musicSlider.value = SoundManager.main.MusicVolume;
        sfxSlider.value = SoundManager.main.SfxVolume;
    }

    public void HandleMusicValueChange(float value)
    {
        SoundManager.main.MusicVolume = value;
    }

    public void HandleSfxValueChange(float value)
    {
        SoundManager.main.SfxVolume = value;
    }
}
