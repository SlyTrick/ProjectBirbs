using System.Collections;
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
        Character collided;
        if (collision.gameObject.TryGetComponent<Character>(out collided))
        {
            if(collided.teamId != teamId)
            {
                Debug.Log("De otro equipo?");

                Destroy(gameObject);
                Instantiate(particleEffect, transform.position, transform.rotation);

                collided.TakeDamage(damage);
            }
            else
            {
                Debug.Log("Del mismo equipo?");

                // Podría haber daño aliado pero daño entre 2
                Destroy(gameObject);
                Instantiate(particleEffect, transform.position, transform.rotation);


                collided.TakeDamage(damage / sameTeamDamage);
            }
            
        }
        else
        {
            Debug.Log("Pared?");
            Destroy(gameObject);
            Instantiate(particleEffect, transform.position, transform.rotation);
        }
        
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
        Instantiate(particleEffect, transform.position, transform.rotation);
    }
}
