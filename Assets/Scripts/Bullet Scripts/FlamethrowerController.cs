using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerController  : BulletController
{
    [SerializeField] private Vector3 scaleRate;
    public override void Start()
    {
        moveVector = Vector3.forward * speed * Time.fixedDeltaTime;
        StartCoroutine(DestroyBullet());
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        transform.localScale += scaleRate;
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
