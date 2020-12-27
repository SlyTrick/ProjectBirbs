using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Character : MonoBehaviourPunCallbacks
{
    #region Variables
    public StateMachine movementSM;
    public GroundedState groundedState;
    public ShootingState shootingState;
    public ShieldState shieldState;
    public DeadState deadState;
    public StunState stunState;
    public FeederState feederState;

    [SerializeField] private float playerAcceleration;
    [SerializeField] private float playerDeceleration;
    [SerializeField] private float shootingAcceleration;
    [SerializeField] private float stunnedAcceleration;
    [SerializeField] private float feederAcceleration;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float shieldCooldown;
    [SerializeField] private float respawnTime;
    [SerializeField] private float stunTime;
    [SerializeField] private int maxLife;
    [SerializeField] private int shieldMaxLife;
    [SerializeField] private float shieldTime;

    public bool onFeeder;
    public int teamId;
    public int life;
    private bool canShoot;
    private bool canShield;
    public int score;
    private int feathers;
    private MatchController matchController;
    private BoxCollider spawnPoint;
    public bool damageable;

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
    [SerializeField] public GameObject[] bulletPrefabs;
    [SerializeField] private GameObject destroyedParticleEffect;

    [SerializeField] private InputController inputController;
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] public PhotonView PV;
    
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
            if(PhotonNetwork.IsConnected && PV.IsMine)
            {
                PV.RPC("Shoot_RPC", RpcTarget.All);
            }
            else
            {
                GameObject objBullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
                objBullet.GetComponent<BulletController>().teamId = teamId;
                objBullet.GetComponent<BulletController>().owner = this;
                Physics.IgnoreCollision(GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
                Physics.IgnoreCollision(shield.GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
            }
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
        if (damageable)
        {
            if (life <= 0)
            {
                damageable = false;
                if (PhotonNetwork.IsConnected)
                {
                    if (PV.IsMine)
                    {
                        PV.RPC("PlayerKilled_RPC", RpcTarget.All, bullet.owner.PV.Owner.ActorNumber);
                    }
                }
                else
                {
                    matchController.PlayerKilled(this, bullet.owner);
                    Instantiate(deathParticleEffect, transform.position, transform.rotation);
                    movementSM.CurrentState.OnDead();
                }
            }
        }
    }

    public void SetPoints(int newScore)
    {
        if (PhotonNetwork.IsConnected)
        {
            PV.RPC("UpdateScore_RPC", RpcTarget.All, newScore);
        }
        else
        {
            score = newScore;
            scoreText.text = "Puntuación: " + score;
        }
    }

    public void Die()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PV.IsMine)
            {
                PV.RPC("Die_RPC", RpcTarget.All);
            }
        }
        else
        {
            GetComponent<CapsuleCollider>().enabled = false;
            firingPoint.SetActive(false);
            sprite.SetActive(false);
            StartCoroutine(Respawn());
        }
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        Vector3 spawnPos = new Vector3(
            Random.Range(spawnPoint.bounds.min.x, spawnPoint.bounds.max.x),
            0,
            Random.Range(spawnPoint.bounds.min.z, spawnPoint.bounds.max.z)
        );
        transform.position = spawnPos;
        transform.forward = spawnPoint.transform.forward;

        movementSM.ChangeState(groundedState);
        if (PhotonNetwork.IsConnected)
        {
            if (PV.IsMine)
            {
                PV.RPC("Respawn_RPC", RpcTarget.All);
            }
        }
        else
        {
            life = maxLife;
            lifeText.text = "Vida: " + life;
            damageable = true;

            GetComponent<CapsuleCollider>().enabled = true;
            sprite.SetActive(true);
            firingPoint.SetActive(true);
        }
    }

    public void CreateShield()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PV.IsMine)
            {
                PV.RPC("CreateShield_RPC", RpcTarget.All);
            }
        }
        else
        {
            shieldController.CreateShield();
        }
    }
    public void RemoveShield()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PV.IsMine)
            {
                PV.RPC("RemoveShield_RPC", RpcTarget.All);
            }
        }
        else
        {
            shieldController.RemoveShield();
            if (!shieldController.parried)
            {
                canShield = false;
                StartCoroutine(ShieldCooldown());
            }
            else
            {
                shieldController.parried = false;
            }
        }
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
        if (!onFeeder)
            movementSM.ChangeState(groundedState);
        else
            movementSM.ChangeState(feederState);
    }
    public void EnterFeeder()
    {
        onFeeder = true;
        if (movementSM.CurrentState != stunState)
            movementSM.ChangeState(feederState);
    }
    public void ExitFeeder()
    {
        onFeeder = false;
        if (movementSM.CurrentState == feederState)
            movementSM.ChangeState(groundedState);
    }
    public void AddFeather()
    {
        feathers++;
    }
    public int LoseFeathers()
    {
        int lostFeathers = feathers / 2;
        feathers -= lostFeathers;
        if (PhotonNetwork.IsConnected)
        {
            PV.RPC("LoseFeathers_RPC", RpcTarget.All, feathers);
        }
        return lostFeathers;
    }

    #region RPCs

    [PunRPC]
    public void Shoot_RPC()
    {
        GameObject objBullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        objBullet.GetComponent<BulletController>().teamId = teamId;
        objBullet.GetComponent<BulletController>().owner = this;
        Physics.IgnoreCollision(GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
        Physics.IgnoreCollision(shield.GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
    }

    [PunRPC]
    public void PlayerKilled_RPC(int killerActorNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            switch (matchController.mode)
            {
                case 0:
                    Character killer = matchController.findByActorNumber(killerActorNumber);
                    matchController.PlayerKilled(this, killer);
                    break;
                case 1:
                    break;
                case 2:
                    matchController.PlayerKilled(this, null);
                    break;
            }
        }
        Instantiate(deathParticleEffect, transform.position, transform.rotation);
        if (PV.IsMine)
        {
            movementSM.CurrentState.OnDead();
        }
    }

    [PunRPC]
    public void Die_RPC()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        firingPoint.SetActive(false);
        sprite.SetActive(false);

        if (PV.IsMine)
        {
            StartCoroutine(Respawn());
        }
    }

    [PunRPC]
    public void Respawn_RPC()
    {
        if (PV.IsMine)
        {
            life = maxLife;
            lifeText.text = "Vida: " + life;
        }
        damageable = true;
        GetComponent<CapsuleCollider>().enabled = true;
        sprite.SetActive(true);
        firingPoint.SetActive(true);
    }

    [PunRPC]
    public void GetSpawnpoint_RPC(int team)
    {
        if (PV.IsMine)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                matchController = FindObjectOfType<MatchController>();
            }
            teamId = team;
            spawnPoint = matchController.GetSpawnPoint(this);
            GetComponent<Transform>().position = spawnPoint.transform.position;
        }
    }

    [PunRPC]
    public void CreateShield_RPC()
    {
        shieldController.CreateShield();
    }

    [PunRPC]
    public void RemoveShield_RPC()
    {
        shieldController.RemoveShield();
        if (PV.IsMine)
        {
            if (!shieldController.parried)
            {
                canShield = false;
                StartCoroutine(ShieldCooldown());
            }
            else
            {
                shieldController.parried = false;
            }
        }
    }

    [PunRPC]
    public void Parry_RPC(int indicePrefab)
    {
        
        GameObject objBullet = Instantiate(bulletPrefabs[indicePrefab], shieldController.transform.position, shieldController.transform.rotation);
        objBullet.GetComponent<BulletController>().teamId = teamId;
        objBullet.GetComponent<BulletController>().damage *= 2;
        objBullet.GetComponent<BulletController>().owner = this;
        objBullet.GetComponent<BulletController>().enabled = true;
        Physics.IgnoreCollision(GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
        Physics.IgnoreCollision(this.GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
    }

    [PunRPC]
    public void DestroyShield_RPC()
    {
        Instantiate(destroyedParticleEffect, shieldController.transform.position, shieldController.transform.rotation);
        if (PV.IsMine)
        {
            movementSM.CurrentState.OnStun();
        }
    }

    [PunRPC]
    public void AddFeather_RPC()
    {
        if(PhotonNetwork.IsMasterClient || PV.IsMine)
        {
            feathers++;
        }
    }

    [PunRPC]
    public void LoseFeathers_RPC(int newFeathers)
    {
        if (PV.IsMine)
        {
            feathers = newFeathers;
        }
    }

    [PunRPC]
    public void UpdateScore_RPC(int newScore)
    {
        if (PV.IsMine)
        {
            score = newScore;
            scoreText.text = "Puntuación: " + score;
        }
    }

    [PunRPC]
    public void UpdateFeederScore_RPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            matchController.UpdateFeederScore(this);
        }
    }

    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
        rigidBody.drag = playerDeceleration;

        life = maxLife;
        score= 0;
        canShoot = true;
        canShield = true;
        teamId = GetComponentInChildren<PlayerInput>().playerIndex;
        damageable = true;

        lifeText.text = "Vida: " + life;
        scoreText.text = "Puntuación: " + score;

        if(PhotonNetwork.IsConnected && PV.IsMine)
        {
            mainCamera.enabled = true;
            playerInput.enabled = true;
        }
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            matchController = FindObjectOfType<MatchController>();
            teamId = matchController.AddPlayer(this);
            PV.RPC("GetSpawnpoint_RPC", RpcTarget.All, teamId);
        }
        else if (!PhotonNetwork.IsConnected)
        {
            mainCamera.enabled = true;
            playerInput.enabled = true;
            matchController = FindObjectOfType<MatchController>();
            matchController.AddPlayer(this);
            spawnPoint = matchController.GetSpawnPoint(this);
        }

        movementSM = new StateMachine();

        groundedState = new GroundedState(this, movementSM);
        shootingState = new ShootingState(this, movementSM);
        shieldState = new ShieldState(this, movementSM);
        deadState = new DeadState(this, movementSM);
        stunState = new StunState(this, movementSM);
        feederState = new FeederState(this, movementSM);
        
        movementSM.Initialize(groundedState);
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!PV.IsMine)
                return;
        }
        movementSM.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!PV.IsMine)
                return;
        }
        movementSM.CurrentState.PhysicsUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!PV.IsMine)
                return;
        }
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

    #region Getters and Setters
    public float GetPlayerAcceleration(){ return playerAcceleration; }
    public float GetPlayerDeceleration(){ return playerDeceleration; }
    public float GetShootingAcceleration(){ return shootingAcceleration; }
    public float GetStunnedAcceleration(){ return stunnedAcceleration; }
    public float GetFeederAcceleration(){ return feederAcceleration; }
    public float GetTimeBetweenShots(){ return timeBetweenShots; }
    public float GetRespawnTime(){ return respawnTime; }
    public int GetMaxLife(){ return maxLife; }
    public int GetShieldMaxLife(){ return shieldMaxLife; }
    public float GetShieldTime(){ return shieldTime; }
    public int GetTeamId(){ return teamId; }
    public int GetLife(){ return life; }
    public bool GetCanShoot(){ return canShoot; }
    public bool GetCanShield(){ return canShield; }
    public int GetScore(){ return score; }
    public int GetFeathers(){ return feathers; }
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
    public MatchController GetMatchController(){ return matchController; }
    #endregion
}
