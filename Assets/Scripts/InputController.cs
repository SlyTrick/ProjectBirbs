using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public  Vector2 leftStickInput;
    public Vector2 rightStickInput;
    public Vector2 lastMousePosition;
    public bool shootInput;
    public Character character;

    public int playerNumber;
    // Update is called once per frame
    void Update()
    {
        /*
        leftStickInput = new Vector2(Input.GetAxisRaw("Horizontal" + playerNumber), Input.GetAxisRaw("Vertical" + playerNumber));
        if (leftStickInput.magnitude > 1)
        {
            leftStickInput.Normalize();
        }
        rightStickInput = new Vector2(Input.GetAxisRaw("R_Horizontal" + playerNumber), Input.GetAxisRaw("R_Vertical" + playerNumber));
        */
        // Si se ha movido el ratón sustituimos rightStickInput, asi se puede usar ambas formas de input a la vez y no hay que cambiar de modo ni nada
        

        //shootInput = Input.GetButton("Shoot" + playerNumber);
    }

    private void OnMove(InputValue value)
    {
        leftStickInput = value.Get<Vector2>();
        if (leftStickInput.magnitude > 1)
        {
            leftStickInput.Normalize();
        }
    }
    private void OnShoot(InputValue value)
    {
        shootInput = value.Get<float>() == 1;
    }
    private void OnLook(InputValue value)
    {
        rightStickInput = value.Get<Vector2>();
    }
    private void OnLookMouse(InputValue value)
    {
        lastMousePosition = value.Get<Vector2>();
        Ray cameraRay = character.mainCamera.ScreenPointToRay(lastMousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        // Comprobar si se llega a cruzar
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            // No sirve ScreenToWorldPoint porque la cámara está inclinada
            // Vector3 pointToLook = mainCamera.ScreenToWorldPoint(new Vector3(lastMousePosition.x, lastMousePosition.y, 5));  
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            // El eje z hay que hacerlo al revés porque el 0,0 en la pantalla es abajo a la izquierda
            rightStickInput = new Vector2(pointToLook.x - character.transform.position.x, pointToLook.z - character.transform.position.z);
        }
    }
}
