using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherController : MonoBehaviour
{
    private MatchController matchController;
    public Rigidbody rigidBody;
    public float acceleration;
    private void Start()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        Character collided;
        if(collision.gameObject.TryGetComponent<Character>(out collided))
        {
            matchController = FindObjectOfType<MatchController>();
            matchController.AddFeather(collided);
            Destroy(gameObject);
        }
    }
}
