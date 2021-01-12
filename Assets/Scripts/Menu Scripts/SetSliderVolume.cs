using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetSliderVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
    public string valueName;

    void Start()
    {
        float value = PlayerPrefs.GetFloat(valueName, 0.75f);
        slider.value = value;
        mixer.SetFloat(valueName, Mathf.Log10(value) * 20);
        Debug.Log("Cargo el valor " + slider.value + " en " + valueName);
    }

    public void SetLevel()
    {
        mixer.SetFloat(valueName, Mathf.Log10(slider.value) * 20);
        
        PlayerPrefs.SetFloat(valueName, slider.value);
        Debug.Log("Guardo el valor " + slider.value + " en " + valueName);
    }
}
