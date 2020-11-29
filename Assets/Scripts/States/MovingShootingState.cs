﻿using System.Collections;
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

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (character.inputController.leftStickInput.magnitude == 0)
            stateMachine.ChangeState(character.idleShooting);

        if (!character.inputController.shootInput)
            stateMachine.ChangeState(character.moving);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}