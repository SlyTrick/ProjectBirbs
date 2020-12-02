using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    #region Variables
    public StateMachine movementSM;
    public IdleState idle;
    public MovingState moving;
    public IdleShootingState idleShooting;
    public MovingShootingState movingShooting;
    public IdleShieldState idleShield;
    public MovingShieldState movingShield;
    public DeadState dead;
    public StunState stun;

    [SerializeField] private float playerAcceleration;
    [SerializeField] private float playerDeceleration;
    [SerializeField] private float shootingAcceleration;
    [SerializeField] private float stunnedAcceleration;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float shieldCooldown;
    [SerializeField] private float respawnTime;
    [SerializeField] private float stunTime;
    [SerializeField] private int maxLife;
    [SerializeField] private int shieldMaxLife;

    private int teamId;
    private int life;
    private bool canShoot;
    private bool canShield;
    private int score;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text lifeText;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject firingPoint;
    [SerializeField] private GameObject deathParticleEffect;
    [SerializeField] private GameObject shield;
    [SerializeField] private ShieldController shieldController;

    [SerializeField] private InputController inputController;
    
    #endregion

    public void Move(Vector2 direction, float acceleration)
    {
        rigidBody.AddForce(new Vector3(direction.x * acceleration * Time.fixedDeltaTime, 0, direction.y * acceleration * Time.fixedDeltaTime), ForceMode.Impulse);
        
    }

    public void Rotate(Vector2 direction)
    {
        if (direction.magnitude > 0f)
        {
            // Queremos que sea en X y en Z
            Vector3 currentRotation = Vector3.right * direction.x + Vector3.forward * direction.y;
            Quaternion playerRotation = Quaternion.LookRotation(currentRotation, Vector3.up);

            rigidBody.rotation = (playerRotation);
        }
    }

    public void Shoot(bool shootInput)
    {
        if(canShoot)
        {
            canShoot = false;
            GameObject objBullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            objBullet.GetComponent<BulletController>().teamId = teamId;
            objBullet.GetComponent<BulletController>().owner = this;
            Physics.IgnoreCollision(GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
            StartCoroutine(ShotCooldown());

        }
    }

    IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    public void TakeDamage(int damage, BulletController bullet)
    {
        life -= damage;
        lifeText.text = "Vida: " + life;

        if (life <= 0)
        {
            bullet.owner.AddPoint();
            Instantiate(deathParticleEffect, transform.position, transform.rotation);
            movementSM.CurrentState.OnDead();
        }
    }

    public void AddPoint()
    {
        score++;
        scoreText.text = "Puntuación: " + score;
    }

    public void Die()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        firingPoint.SetActive(false);
        sprite.SetActive(false);
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        transform.position = new Vector3(0,0,0);
        movementSM.ChangeState(idle);

        life = maxLife;
        lifeText.text = "Vida: " + life;

        GetComponent<CapsuleCollider>().enabled = true;
        sprite.SetActive(true);
        firingPoint.SetActive(true);

    }

    public void CreateShield()
    {
        shieldController.CreateShield();
    }
    public void RemoveShield()
    {
        shieldController.RemoveShield();
        canShield = false;
        StartCoroutine(ShieldCooldown());

    }

    IEnumerator ShieldCooldown()
    {
        yield return new WaitForSeconds(shieldCooldown);
        canShield = true;
    }
    public void Stun()
    {
        StartCoroutine(StunCooldown());
    }

    public IEnumerator StunCooldown()
    {
        yield return new WaitForSeconds(stunTime);
        shieldController.RestoreLife();
        movementSM.ChangeState(idle);
    }

    #region MonoBehaviour Callbacks
    private void Start()
    {
        rigidBody.drag = playerDeceleration;

        life = maxLife;
        score= 0;
        canShoot = true;
        canShield = true;
        teamId = GetComponentInChildren<PlayerInput>().playerIndex;

        lifeText.text = "Vida: " + life;
        scoreText.text = "Puntuación: " + score;

        movementSM = new StateMachine();

        idle = new IdleState(this, movementSM);
        moving = new MovingState(this, movementSM);
        idleShooting = new IdleShootingState(this, movementSM);
        movingShooting = new MovingShootingState(this, movementSM);
        idleShield = new IdleShieldState(this, movementSM);
        movingShield= new MovingShieldState(this, movementSM);
        dead = new DeadState(this, movementSM);
        stun = new StunState(this, movementSM);
        
        movementSM.Initialize(idle);
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

    #region Getters and Setters
    public float GetPlayerAcceleration(){ return playerAcceleration; }
    public float GetPlayerDeceleration(){ return playerDeceleration; }
    public float GetShootingAcceleration(){ return shootingAcceleration; }
    public float GetStunnedAcceleration(){ return stunnedAcceleration; }
    public float GetTimeBetweenShots(){ return timeBetweenShots; }
    public float GetRespawnTime(){ return respawnTime; }
    public int GetMaxLife(){ return maxLife; }
    public int GetShieldMaxLife(){ return shieldMaxLife; }
    public int GetTeamId(){ return teamId; }
    public int GetLife(){ return life; }
    public bool GetCanShoot(){ return canShoot; }
    public bool GetCanShield(){ return canShield; }
    public int GetScore(){ return score; }
    public Text GetScoreText(){ return scoreText; }
    public Text GetLifeText(){ return lifeText; }
    public Camera GetCamera(){ return mainCamera; }
    public Rigidbody GetRigidBody(){ return rigidBody; }
    public GameObject GetBulletPrefab(){ return bulletPrefab; }
    public GameObject GetSprite(){ return sprite; }
    public Transform GetFirePoint(){ return firePoint; }
    public GameObject GetFiringPoint(){ return firingPoint; }
    public GameObject GetDeathParticleEffect(){ return deathParticleEffect; }
    public GameObject GetShield(){ return shield; }
    public InputController GetInputController(){ return inputController; }
    #endregion
}
