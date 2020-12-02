using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingShootingState : ShootingState
{
    public MovingShootingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
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
    }

    public override void OnMove()
    {
        base.OnMove();
        if (character.GetInputController().leftStickInput.magnitude == 0)
            stateMachine.ChangeState(character.idleShooting);
    }

    public override void OnShoot()
    {
        base.OnShoot();
        if (!character.GetInputController().shootInput)
            stateMachine.ChangeState(character.moving);
    }
    public override void OnShield()
    {
        base.OnShield();
        if (character.GetInputController().shieldInput && character.GetCanShield())
            stateMachine.ChangeState(character.movingShield);
    }
    public override void OnLook()
    {
        base.OnLook();
    }
    public override void OnDead()
    {
        base.OnDead();
    }
    public override void OnStun()
    {
        base.OnStun();
    }
}
