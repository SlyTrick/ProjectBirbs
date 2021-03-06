﻿using System.Collections;
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

    public int teamId;
    public Character owner;

    public GameObject particleEffect;
    public GameObject bulletGraphics;

    [SerializeField] public GameObject bulletWallSoundPrefab;
    // Start is called before the first frame update
    public virtual void Start()
    {
        moveVector = Vector3.forward * speed * Time.fixedDeltaTime;
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        transform.Translate(moveVector);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 11) //Las paredes estan en la layer 11
        {
            Instantiate(bulletWallSoundPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        Instantiate(particleEffect, transform.position, transform.rotation);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
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
