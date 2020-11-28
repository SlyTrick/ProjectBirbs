using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : State
{
    public Vector2 leftStickInput;
    private Vector2 rightStickInput;
    private Vector3 lastMousePosition;

    public GroundedState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        leftStickInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (leftStickInput.magnitude > 1)
        {
            leftStickInput.Normalize();
        }
        rightStickInput = new Vector2(Input.GetAxisRaw("R_Horizontal"), Input.GetAxisRaw("R_Vertical"));

        // Si se ha movido el ratón sustituimos rightStickInput, asi se puede usar ambas formas de input a la vez y no hay que cambiar de modo ni nada
        if (lastMousePosition != Input.mousePosition)
        {
            lastMousePosition = Input.mousePosition;

            Ray cameraRay = character.mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;
            // Comprobar si se llega a cruzar
            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                // No sirve ScreenToWorldPoint porque la cámara está inclinada
                // Vector3 pointToLook = mainCamera.ScreenToWorldPoint(new Vector3(lastMousePosition.x, lastMousePosition.y, 5));  
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                // El eje z hay que hacerlo al revés porque el 0,0 en la pantalla es abajo a la izquierda
                rightStickInput = new Vector2(pointToLook.x - character.transform.position.x, -pointToLook.z + character.transform.position.z);
            }
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Rotate(rightStickInput);
    }
}
