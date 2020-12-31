using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FeederController : MonoBehaviour
{
    private BoxCollider box;
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Character target;
        if (other.gameObject.TryGetComponent<Character>(out target))
        {
            target.EnterFeeder();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Character target;
        if (other.gameObject.TryGetComponent<Character>(out target))
        {
            target.ExitFeeder();
        }
    }
}
