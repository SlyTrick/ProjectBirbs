using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    #region Variables
    public StateMachine movementSM;
    public IdleState idle;
    public MovingState moving;
    public IdleShootingState idleShooting;
    public MovingShootingState movingShooting;

    public Camera mainCamera;
    public float playerAcceleration;
    public float shootingAcceleration;
    public float playerDeceleration;
    private Rigidbody rb;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float timeBetweenShots;
    public int teamId;
    private bool canShoot;


    public InputController inputController;
    
    #endregion

    public void Move(Vector2 direction, float acceleration)
    {
        rb.AddForce(new Vector3(direction.x * acceleration * Time.fixedDeltaTime, 0, direction.y * acceleration * Time.fixedDeltaTime), ForceMode.Impulse);
        
    }

    public void Rotate(Vector2 direction)
    {
        if (direction.magnitude > 0f)
        {
            // Queremos que sea en X y en Z
            Vector3 currentRotation = Vector3.right * direction.x + Vector3.forward * direction.y;
            Quaternion playerRotation = Quaternion.LookRotation(currentRotation, Vector3.up);

            rb.rotation = (playerRotation);
        }
    }

    public void Shoot(bool shootInput)
    {
        if(shootInput && canShoot)
        {
            canShoot = false;
            GameObject objBullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            objBullet.GetComponent<BulletController>().teamId = teamId;
            Physics.IgnoreCollision(GetComponent<Collider>(), objBullet.GetComponentInChildren<Collider>());
            StartCoroutine(ShotCooldown());

        }

    }

    IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    #region MonoBehaviour Callbacks
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = playerDeceleration;

        canShoot = true;
        teamId = GetComponentInChildren<PlayerInput>().playerIndex;
        movementSM = new StateMachine();

        idle = new IdleState(this, movementSM);
        moving = new MovingState(this, movementSM);
        idleShooting = new IdleShootingState(this, movementSM);
        movingShooting = new MovingShootingState(this, movementSM);
        
        movementSM.Initialize(idle);
    }

    private void Update()
    {
        movementSM.CurrentState.HandleInput();

        movementSM.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        movementSM.CurrentState.PhysicsUpdate();
    }
    #endregion
}
