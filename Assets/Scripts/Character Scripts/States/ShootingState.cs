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

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Move(character.GetInputController().leftStickInput, character.GetShootingAcceleration());
        character.Rotate(character.GetInputController().rightStickInput);
        character.Shoot(character.GetInputController().shootInput);
    }

    public override void OnMove()
    {
        base.OnMove();
    }

    public override void OnShoot()
    {
        base.OnShoot();
    }
    public override void OnLook()
    {
        base.OnLook();
    }
    public override void OnDead()
    {
        base.OnDead();
        stateMachine.ChangeState(character.dead);
    }
    public override void OnStun()
    {
        base.OnStun();
    }
}
