using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundControler : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioMixer masterMixer;

    public Slider bgmSlider;
    public Slider sfxSlider;

    static float bgmSound = -20.0f;

    static float sfxSound = -20.0f;

    void Start()
    {
        bgmSlider.value = bgmSound;
        sfxSlider.value = sfxSound;
    }

        void Update()
    {
        BGMAudioControl();
        SFXAudioControl();
    }

    public void BGMAudioControl()
    {
        bgmSound = bgmSlider.value;

        if (bgmSound == -40f) masterMixer.SetFloat("BGM", -80);
        else masterMixer.SetFloat("BGM", bgmSound);
    }

    public void SFXAudioControl()
    {
        sfxSound = sfxSlider.value;

        if (sfxSound == -40f) masterMixer.SetFloat("SFX", -80);
        else masterMixer.SetFloat("SFX", sfxSound);
    }

}
