using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public string valueName;

    void Start()
    {
        float value = PlayerPrefs.GetFloat(valueName, 0.75f);
        mixer.SetFloat(valueName, Mathf.Log10(value) * 20);
        Debug.Log("Cargo el valor " + value + " en " + valueName);
    }
}
