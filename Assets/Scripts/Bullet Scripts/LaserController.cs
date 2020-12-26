using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : BulletController
{
    [SerializeField]private int maxRicochet;
    private int numRicochet;
    public override void Start()
    {
        
        base.Start();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void OnCollisionEnter(Collision collision)
    {
        if (numRicochet >= maxRicochet || collision.gameObject.GetComponent<Character>() || collision.gameObject.GetComponent<ShieldController>())
        {
            base.OnCollisionEnter(collision);
        }
        else
        {
            numRicochet++;
            transform.forward = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
        }
        
    }
}
