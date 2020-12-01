﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damage;
    public int sameTeamDamage;
    public float speed;
    public float timeToLive;
    Vector3 moveVector;

    public int teamId;
    public Character owner;

    public GameObject particleEffect;
    // Start is called before the first frame update
    void Start()
    {
        moveVector = Vector3.forward * speed * Time.fixedDeltaTime;
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Translate(moveVector);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Instantiate(particleEffect, transform.position, transform.rotation);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
        Instantiate(particleEffect, transform.position, transform.rotation);
    }
}
