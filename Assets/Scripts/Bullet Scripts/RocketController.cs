using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class RocketController : BulletController
{
    [SerializeField] private GameObject explosion;
    public override void Start()
    {
        if (PhotonNetwork.IsConnected && !PV.IsMine)
        {
            return;
        }
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
            if (PhotonNetwork.IsConnected && PV.IsMine)
            {
                GameObject objExplosion = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Bullets", explosion.name), transform.position, transform.rotation);
                objExplosion.GetComponent<BulletController>().teamId = teamId;
                objExplosion.GetComponent<BulletController>().owner = owner;
            }
            else
            {
                GameObject objExplosion = Instantiate(explosion, transform.position, transform.rotation);
                objExplosion.GetComponent<BulletController>().teamId = teamId;
                objExplosion.GetComponent<BulletController>().owner = owner;
            }
        }
        base.OnCollisionEnter(collision);
    }

    IEnumerator CreateExplosion()
    {
        yield return new WaitForSeconds(timeToLive);
        if (PhotonNetwork.IsConnected && PV.IsMine)
        {
            GameObject objExplosion = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Bullets", explosion.name), transform.position, transform.rotation);
            objExplosion.GetComponent<BulletController>().teamId = teamId;
            objExplosion.GetComponent<BulletController>().owner = owner;
        }
        else
        {
            GameObject objExplosion = Instantiate(explosion, transform.position, transform.rotation);
            objExplosion.GetComponent<BulletController>().teamId = teamId;
            objExplosion.GetComponent<BulletController>().owner = owner;
        }
    }
}
