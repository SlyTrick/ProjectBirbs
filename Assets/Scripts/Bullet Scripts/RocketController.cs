using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : BulletController
{
    [SerializeField] private GameObject explosion;
    public override void Start()
    {
        StartCoroutine(CreateExplosion());
        base.Start();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.GetComponent<ShieldController>())
        {
            GameObject objExplosion = Instantiate(explosion, transform.position, transform.rotation);
            objExplosion.GetComponent<BulletController>().teamId = teamId;
            objExplosion.GetComponent<BulletController>().owner = owner;
        }
        base.OnCollisionEnter(collision);
    }

    IEnumerator CreateExplosion()
    {
        yield return new WaitForSeconds(timeToLive);
        GameObject objExplosion = Instantiate(explosion, transform.position, transform.rotation);
        objExplosion.GetComponent<BulletController>().teamId = teamId;
        objExplosion.GetComponent<BulletController>().owner = owner;
    }
}
