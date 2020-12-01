using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public Vector2 leftStickInput;
    public Vector2 rightStickInput;
    public Vector2 lastMousePosition;
    public bool shootInput;
    public Character character;

    public int playerNumber;
    
    // Sale un error rojo de unity al pulsar un botón para añadir el personaje.
    // Eso es porque se crea el personaje y detecta un input antes de crear la máquina de estados.
    // En el juego final no daría error, porque se crearía el personaje por un evento distinto y no recibiría inputs hasta después
    // Por si acaso por ahora dejo un if !null
    private void OnMove(InputValue value)
    {
        leftStickInput = value.Get<Vector2>();
        if (leftStickInput.magnitude > 1)
        {
            leftStickInput.Normalize();
        }
        if (character.movementSM != null)
        {
            character.movementSM.CurrentState.OnMove();
        }
        

    }
    private void OnShoot(InputValue value)
    {
        shootInput = value.Get<float>() == 1;

        if (character.movementSM != null)
        {
            character.movementSM.CurrentState.OnShoot();
        }
    }
    private void OnLook(InputValue value)
    {
        rightStickInput = value.Get<Vector2>();

        if (character.movementSM != null)
        {
            character.movementSM.CurrentState.OnLook();
        }
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

        if (character.movementSM != null)
        {
            character.movementSM.CurrentState.OnLook();
        }
    }
}
