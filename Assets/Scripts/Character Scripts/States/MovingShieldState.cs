using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingShieldState : ShieldState
{

    public MovingShieldState(Character character, StateMachine stateMachine) : base(character, stateMachine)
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
            stateMachine.ChangeState(character.idleShield);
    }

    public override void OnShoot()
    {
        base.OnShoot();
    }
    public override void OnShield()
    {
        base.OnShield();
        if (!character.GetInputController().shieldInput)
            stateMachine.ChangeState(character.moving);
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
