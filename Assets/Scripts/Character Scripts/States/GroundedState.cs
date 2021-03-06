﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : State
{

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


    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Move(character.GetInputController().leftStickInput, character.GetPlayerAcceleration());
        character.Rotate(character.GetInputController().rightStickInput);
    }

    public override void OnMove()
    {
        base.OnMove();
    }

    public override void OnShoot()
    {
        base.OnShoot();
        if (character.GetInputController().shootInput)
            stateMachine.ChangeState(character.shootingState);
    }
    public override void OnShield()
    {
        base.OnShield();
        if (character.GetInputController().shieldInput && character.GetCanShield())
            stateMachine.ChangeState(character.shieldState);
    }
    public override void OnLook()
    {
        base.OnLook();
    }
    public override void OnDead()
    {
        base.OnDead();
        stateMachine.ChangeState(character.deadState);
    }
    public override void OnStun()
    {
        base.OnStun();
    }
}
