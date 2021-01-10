using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Dummy : Character
{
    private MatchController matchController;
    private BoxCollider spawnPoint;

    public new void Shoot()
    {
        if (GetCanShoot())
        {
            SetCanShoot(false);
            GameObject objBullet = Instantiate(GetBulletPrefab(), GetFirePoint().position, transform.rotation);
            objBullet.GetComponent<BulletController>().teamId = teamId;
            objBullet.GetComponent<BulletController>().owner = this;
            Physics.IgnoreCollision(GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
            
            StartCoroutine(ShotCooldown());
        }
    }

    IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(GetTimeBetweenShots());
        SetCanShoot(true);
    }

    public new void TakeDamage(int damage, BulletController bullet)
    {
        Debug.Log(damage);
    }



    #region MonoBehaviour Callbacks
    private void Start()
    {
        GetRigidBody().drag = GetPlayerDeceleration();

        life = 1;
        score = 0;
        SetCanShoot(true);
        damageable = true;

        matchController = FindObjectOfType<MatchController>();
        matchController.AddPlayer(this);
        spawnPoint = matchController.GetSpawnPoint(this);

        teamId = 5;

        movementSM = new StateMachine();

        groundedState = new GroundedState(this, movementSM);
        shootingState = new ShootingState(this, movementSM);
        shieldState = new ShieldState(this, movementSM);
        deadState = new DeadState(this, movementSM);
        stunState = new StunState(this, movementSM);
        feederState = new FeederState(this, movementSM);

        movementSM.Initialize(shootingState);
    }

    private void Update()
    {
        movementSM.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        movementSM.CurrentState.PhysicsUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        BulletController collided;
        if (collision.gameObject.TryGetComponent<BulletController>(out collided))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collided.GetComponentInChildren<Collider>());
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
    #endregion
}
