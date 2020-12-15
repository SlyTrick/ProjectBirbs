using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : BulletController
{
    public override void Start()
    {
        StartCoroutine(DestroyBullet());
    }
    public override void FixedUpdate()
    {

    }
    public override void OnCollisionEnter(Collision collision)
    {

    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    }
}
