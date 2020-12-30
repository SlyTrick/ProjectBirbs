using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBarrierController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.forward = Vector3.Reflect(collision.transform.forward, collision.GetContact(0).normal);
        Debug.Log("nigger");
    }
}
