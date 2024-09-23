using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    private Slider soundSlider;
    [SerializeField]
    private Slider musicSlider;

    private void Start()
    {
        //musicSlider.value = SoundManager.main.backgroundAudio.volume;
        //soundSlider.value = SoundManager.main.sfxAudio.volume;
    }

    private void Update()
    {
        //SoundManager.main.backgroundAudio.volume = musicSlider.value;
        //SoundManager.main.sfxAudio.volume = soundSlider.value;
    }
}
