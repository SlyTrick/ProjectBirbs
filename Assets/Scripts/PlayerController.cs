using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    public float playerAcceleration;

    private Rigidbody rb;
    private Vector2 leftStickInput;
    private Vector2 rightStickInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        leftStickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rightStickInput = new Vector2(Input.GetAxis("R_Horizontal"), Input.GetAxis("R_Vertical"));
    }

    private void FixedUpdate()
    {
        Vector3 currentMovement = new Vector3(leftStickInput.x * playerSpeed * Time.deltaTime, 0, leftStickInput.y * playerSpeed * Time.deltaTime);

        //rb.velocity.Set(Mathf.Lerp(rb.velocity.x, currentMovement.x * playerSpeed, playerAcceleration), Mathf.Lerp(rb.velocity.y, currentMovement.y * playerSpeed, playerAcceleration));
        rb.MovePosition(rb.position + currentMovement);

        if(rightStickInput.magnitude > 0f)
        {
            // Queremos que sea en X y en Z
            Vector3 currentRotation = Vector3.left * rightStickInput.x + Vector3.forward * rightStickInput.y;
            Quaternion playerRotation = Quaternion.LookRotation(currentRotation, Vector3.up);

            rb.rotation = (playerRotation);

            Debug.Log(currentRotation);
        }
    }
}
