using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public int life;
    private int maxLife;
    private int teamId;
    private BoxCollider boxCollider;
    private SpriteRenderer sprite;


    [SerializeField] private GameObject birb;
    [SerializeField] private float shieldSize;
    [SerializeField] private float rechargeRate;
    [SerializeField] private float rechargeStart;
    [SerializeField] private int recharge;
    private Character birbCharacter;
    [SerializeField] private GameObject destroyedParticleEffect;
    private void TakeDamage(int damage, BulletController bullet)
    {
        life -= damage;
        Debug.Log("Escudo " + teamId + ": Tengo " + life + " de vida");
        transform.localScale = new Vector3(shieldSize * ((float)life / maxLife), transform.localScale.y, transform.localScale.z);

        if (life <= 0)
        {
            Instantiate(destroyedParticleEffect, transform.position, transform.rotation);
            birbCharacter.movementSM.CurrentState.OnStun();
        }
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
        sprite.enabled = true;
        CancelInvoke();
    }

    public void RemoveShield()
    {
        boxCollider.enabled = false;
        sprite.enabled = false;
        InvokeRepeating("RechargeLife", rechargeStart, rechargeRate);
    }
    // Start is called before the first frame update
    void Start()
    {
        birbCharacter = birb.GetComponent<Character>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        maxLife = birbCharacter.GetShieldMaxLife();
        life = maxLife;
        teamId = birbCharacter.GetTeamId();
        transform.localScale = new Vector3(shieldSize, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    /*private void Update()
    {
        transform.rotation = birb.transform.rotation;

        transform.position = new Vector3(birb.transform.position.x, birb.transform.position.y, birb.transform.position.z) + transform.forward * 0.3f;
    }
    */
    private void OnCollisionEnter(Collision collision)
    {
        BulletController collided;
        if (collision.gameObject.TryGetComponent<BulletController>(out collided))
        {
            if (collided.teamId != teamId)
            {
                //Debug.Log("De otro equipo?");
                TakeDamage(collided.damage, collided);
            }
            else
            {
                //Debug.Log("Del mismo equipo?");

                // Podría haber daño aliado pero daño entre 2
                TakeDamage(collided.damage / collided.sameTeamDamage, collided);
            }

        }
    }
}
