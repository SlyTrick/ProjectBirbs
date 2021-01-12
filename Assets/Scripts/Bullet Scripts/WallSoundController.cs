using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSoundController : MonoBehaviour
{
    [SerializeField] public AudioSource sonido;

    void Start()
    {
        StartCoroutine(timeToLive());
    }

    IEnumerator timeToLive()
    {
        yield return new WaitForSeconds(sonido.clip.length);
        Destroy(this.gameObject);
    }
}
