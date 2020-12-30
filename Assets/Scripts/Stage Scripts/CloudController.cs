using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CloudController : MonoBehaviour
{
    private Vector3 moveVector;
    [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        moveVector = Vector3.forward * speed * Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        transform.Translate(moveVector);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character birb))
        {
            SpriteRenderer birbGraphics = birb.GetComponentInChildren<SpriteRenderer>();
            birbGraphics.enabled = false;
        }
        else if (other.TryGetComponent<BulletController>(out BulletController bullet))
        {
            SpriteRenderer bulletGraphics = bullet.GetComponentInChildren<SpriteRenderer>();
            bulletGraphics.enabled = false;
        }
        else if (other.TryGetComponent<ShieldController>(out ShieldController shield))
        {
            SpriteRenderer shieldGraphics = shield.GetComponentInChildren<SpriteRenderer>();
            shieldGraphics.enabled = false;
        }
        else if(other.TryGetComponent<StageBarrierController>(out StageBarrierController stageBarrier))
        {
            transform.forward = Vector3.Reflect(transform.forward, other.transform.forward);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character birb))
        {
            SpriteRenderer birbGraphics = birb.GetComponentInChildren<SpriteRenderer>();
            birbGraphics.enabled = true;
        }
        else if (other.TryGetComponent<BulletController>(out BulletController bullet))
        {
            SpriteRenderer bulletGraphics = bullet.GetComponentInChildren<SpriteRenderer>();
            bulletGraphics.enabled = true;
        }
        else if (other.TryGetComponent<ShieldController>(out ShieldController shield))
        {
            SpriteRenderer shieldGraphics = shield.GetComponentInChildren<SpriteRenderer>();
            shieldGraphics.enabled = true;
        }
    }
}
