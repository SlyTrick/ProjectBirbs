using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour
{
    public int damage;
    public int sameTeamDamage;
    public float speed;
    public float timeToLive;
    public Vector3 moveVector;
    [SerializeField] public string nombre;
    [SerializeField] public PhotonView PV;

    public int teamId;
    public Character owner;

    public GameObject particleEffect;
    public GameObject bulletGraphics;
    // Start is called before the first frame update
    public virtual void Start()
    {
        if(PhotonNetwork.IsConnected && !PV.IsMine)
        {
            return;
        }
        moveVector = Vector3.forward * speed * Time.fixedDeltaTime;
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected && !PV.IsMine)
        {
            return;
        }
        transform.Translate(moveVector);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsConnected && !PV.IsMine)
        {
            return;
        }
        if (PV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Instantiate(particleEffect, transform.position, transform.rotation);
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
        Instantiate(particleEffect, transform.position, transform.rotation);
    }

    public void SetVisible()
    {
        bulletGraphics.SetActive(true);
    }
    public void SetInvisible()
    {
        bulletGraphics.SetActive(false);
    }
}
