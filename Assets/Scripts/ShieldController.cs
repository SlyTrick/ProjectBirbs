﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ShieldController : MonoBehaviour
{
    public int life;
    public bool parried;
    private int maxLife;
    private float shieldTime;
    private float damageRate;
    private int teamId;
    private float elapsedTime;
    
    private BoxCollider boxCollider;

    public GameObject shieldGraphics;
    [SerializeField] private GameObject birb;
    [SerializeField] private float shieldSize;
    [SerializeField] private float rechargeRate;
    [SerializeField] private float rechargeStart;
    [SerializeField] private int recharge;
    [SerializeField] private float parryTime;
    private Character birbCharacter;
    [SerializeField] private GameObject destroyedParticleEffect;

    [SerializeField] private PhotonView PV;
    [SerializeField] public AudioSource sShield;

    private void TakeDamage(int damage)
    {
        life -= damage;
        Debug.Log("Escudo " + teamId + ": Tengo " + life + " de vida");
        transform.localScale = new Vector3(shieldSize * ((float)life / maxLife), transform.localScale.y, transform.localScale.z);

        if (life <= 0)
        {
            if (PhotonNetwork.IsConnected)
            {
                PV.RPC("DestroyShield_RPC", RpcTarget.All);
            }
            else
            {
                DestroyShield();
            }
        }
    }

    private void LoseLife()
    {
        life -= 1;
        transform.localScale = new Vector3(shieldSize * ((float)life / maxLife), transform.localScale.y, transform.localScale.z);
        if (life <= 0)
        {
            if (PhotonNetwork.IsConnected)
            {
                PV.RPC("DestroyShield_RPC", RpcTarget.All);
                CancelInvoke();
            }
            else
            {
                DestroyShield();
                CancelInvoke();
            }
        }
    }
    
    private void DestroyShield()
    {
        Instantiate(destroyedParticleEffect, transform.position, transform.rotation);
        birbCharacter.movementSM.CurrentState.OnStun();
    }
    
    public void RestoreLife()
    {
        life = maxLife;
    }

    private void RechargeLife()
    {
        life += recharge;
        if(life >= maxLife)
        {
            life = maxLife;
            CancelInvoke();
        }
    }

    public void CreateShield()
    {
        transform.localScale = new Vector3(shieldSize * ((float)life / maxLife), transform.localScale.y, transform.localScale.z);
        boxCollider.enabled = true;
        SetVisible();

        elapsedTime = 0;
        // Cancelamos la recuperación de vida
        CancelInvoke();
        InvokeRepeating("LoseLife", 0, damageRate);
    }

    public void RemoveShield()
    {
        boxCollider.enabled = false;
        SetInvisible();
        // Cancelamos la pérdida de vida
        CancelInvoke();
        InvokeRepeating("RechargeLife", rechargeStart, rechargeRate);
    }

    public void SetVisible()
    {
        shieldGraphics.SetActive(true);
    }
    public void SetInvisible()
    {
        shieldGraphics.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        birbCharacter = birb.GetComponent<Character>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        maxLife = birbCharacter.GetShieldMaxLife();
        shieldTime = birbCharacter.GetShieldTime();
        life = maxLife;
        teamId = birbCharacter.GetTeamId();
        transform.localScale = new Vector3(shieldSize, transform.localScale.y, transform.localScale.z);

        // En un segundo tiene que perder maxLife / shieldTime, se hace cada damageRate
        damageRate = 1 / (maxLife / shieldTime);
        elapsedTime = parryTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if(elapsedTime < parryTime)
        {
            elapsedTime += Time.deltaTime;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        BulletController collided;
        if (collision.gameObject.TryGetComponent<BulletController>(out collided) && collided.speed != 0)
        {
            if (!sShield.isPlaying) {
                sShield.Play();
            }
            if (PhotonNetwork.IsConnected)
            {
                if (!PV.IsMine)
                {
                    return;
                }
            }
            // Si todavia no ha pasado el tiempo, rebota, si ha pasado se recibe daño
            if (elapsedTime < parryTime)
            {
                parried = true;
                if (PhotonNetwork.IsConnected)
                {
                    int indicePrefabBala = 0;
                    for(int i = 0; i < birbCharacter.bulletPrefabs.Length; i++)
                    {
                        if(birbCharacter.bulletPrefabs[i].GetComponent<BulletController>().nombre == collided.nombre)
                        {
                            indicePrefabBala = i;
                            break;
                        }
                    }
                    PV.RPC("Parry_RPC", RpcTarget.All, indicePrefabBala);
                }
                else
                {
                    GameObject objBullet = Instantiate(collision.gameObject, transform.position, transform.rotation);
                    objBullet.GetComponent<BulletController>().teamId = teamId;
                    objBullet.GetComponent<BulletController>().damage *= 2;
                    objBullet.GetComponent<BulletController>().owner = birbCharacter;
                    objBullet.GetComponent<BulletController>().enabled = true;
                    Physics.IgnoreCollision(GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
                    Physics.IgnoreCollision(birb.GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
                }
            }
            else
            {
                parried = false;
                if (collided.teamId != teamId)
                {
                    //Debug.Log("De otro equipo?");
                    TakeDamage(collided.damage);
                }
                else
                {
                    //Debug.Log("Del mismo equipo?");

                    // Podría haber daño aliado pero daño entre 2
                    TakeDamage(collided.damage / collided.sameTeamDamage);
                }
            }
        }
    }
    
}
