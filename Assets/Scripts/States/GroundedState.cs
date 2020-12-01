using System.Collections;
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
        if (character.life <= 0)
            stateMachine.ChangeState(character.dead);

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Move(character.inputController.leftStickInput, character.playerAcceleration);
        character.Rotate(character.inputController.rightStickInput);
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
        base.OnLook();
        stateMachine.ChangeState(character.dead);
    }
}
