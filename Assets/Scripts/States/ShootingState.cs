using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : State
{

    public ShootingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
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
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Move(character.inputController.leftStickInput, character.shootingAcceleration);
        character.Rotate(character.inputController.rightStickInput);
        character.Shoot(character.inputController.shootInput);
    }
}
