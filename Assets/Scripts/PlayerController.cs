﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public float playerSpeed;
    public float playerAcceleration;
    public float playerDeceleration;

    private Rigidbody rb;
    private Vector2 leftStickInput;
    private Vector2 rightStickInput;
    private Vector3 lastMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = playerDeceleration;
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        leftStickInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(leftStickInput.magnitude > 1)
        {
            leftStickInput.Normalize();
        }
        rightStickInput = new Vector2(Input.GetAxisRaw("R_Horizontal"), Input.GetAxisRaw("R_Vertical"));

        // Si se ha movido el ratón sustituimos rightStickInput, asi se puede usar ambas formas de input a la vez y no hay que cambiar de modo ni nada
        if(lastMousePosition != Input.mousePosition)
        {
            lastMousePosition = Input.mousePosition;

            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;
            // Comprobar si se llega a cruzar
            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                // No sirve ScreenToWorldPoint porque la cámara está inclinada
                // Vector3 pointToLook = mainCamera.ScreenToWorldPoint(new Vector3(lastMousePosition.x, lastMousePosition.y, 5));  
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                // El eje z hay que hacerlo al revés porque el 0,0 en la pantalla es abajo a la izquierda
                rightStickInput = new Vector2(pointToLook.x - transform.position.x, -pointToLook.z + transform.position.z);
            }
        }
    }

    private void FixedUpdate()
    {
        //Vector3 currentMovement = new Vector3(leftStickInput.x * playerSpeed * Time.deltaTime, 0, leftStickInput.y * playerSpeed * Time.deltaTime);
        //rb.MovePosition(rb.position + currentMovement);
        //rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, leftStickInput.x * playerSpeed, playerAcceleration), 0, Mathf.Lerp(rb.velocity.z, leftStickInput.y * playerSpeed, playerAcceleration));
        // Debug.Log(rb.velocity);
        rb.AddForce(new Vector3(leftStickInput.x * playerAcceleration * Time.fixedDeltaTime, 0, leftStickInput.y * playerAcceleration * Time.fixedDeltaTime), ForceMode.Impulse);

        if (rightStickInput.magnitude > 0f)
        {
            // Queremos que sea en X y en Z
            Vector3 currentRotation = Vector3.right * rightStickInput.x + Vector3.back * rightStickInput.y;
            Quaternion playerRotation = Quaternion.LookRotation(currentRotation, Vector3.up);

            rb.rotation = (playerRotation);
        }
    }

}
