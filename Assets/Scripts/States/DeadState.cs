using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : GroundedState
{
    public DeadState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        character.GetComponent<CapsuleCollider>().enabled = false;
        character.StartCoroutine(character.Respawn());
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
    }
}
