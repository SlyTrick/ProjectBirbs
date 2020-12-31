using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : BulletController
{
    private Rigidbody rigidBody;
    [SerializeField] private float changeDirectionTime;
    

    public override void Start()
    {
        
        base.Start();
        rigidBody = GetComponent<Rigidbody>();
        StartCoroutine(ChangeDirection());
    }
    public override void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        rigidBody.AddForce(transform.forward * speed, ForceMode.Acceleration);
    }
    public override void OnCollisionEnter(Collision collision)
    { 
            base.OnCollisionEnter(collision);

    }

    IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(changeDirectionTime);
        transform.forward = -transform.forward;
    }
}
