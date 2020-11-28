using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Variables
    public StateMachine movementSM;
    public IdleState idle;
    public MovingState moving;

    public Camera mainCamera;
    public float playerAcceleration;
    public float playerDeceleration;
    private Rigidbody rb;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float timeBetweenShots;
    private bool canShoot;  
    #endregion

    public void Move(Vector2 direction)
    {
        rb.AddForce(new Vector3(direction.x * playerAcceleration * Time.fixedDeltaTime, 0, direction.y * playerAcceleration * Time.fixedDeltaTime), ForceMode.Impulse);
        
    }

    public void Rotate(Vector2 direction)
    {
        if (direction.magnitude > 0f)
        {
            // Queremos que sea en X y en Z
            Vector3 currentRotation = Vector3.right * direction.x + Vector3.back * direction.y;
            Quaternion playerRotation = Quaternion.LookRotation(currentRotation, Vector3.up);

            rb.rotation = (playerRotation);
        }
    }

    public void Shoot(bool shootInput)
    {
        if(shootInput && canShoot)
        {
            canShoot = false;
            Instantiate(bulletPrefab, firePoint.position, transform.rotation);
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

        movementSM = new StateMachine();

        idle = new IdleState(this, movementSM);
        moving = new MovingState(this, movementSM);

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
