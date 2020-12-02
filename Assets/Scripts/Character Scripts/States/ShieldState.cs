using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldState : State
{

    public ShieldState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        character.CreateShield();
    }

    public override void Exit()
    {
        base.Exit();
        character.RemoveShield();
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
        stateMachine.ChangeState(character.stun);
    }
}
