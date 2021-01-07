using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExplosionController : BulletController
{
    public override void Start()
    {
        if (PhotonNetwork.IsConnected && !PV.IsMine)
        {
            return;
        }
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
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
