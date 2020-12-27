using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FeederState : State
{
    private float feederRate;
    private float counter;
    public FeederState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        counter = 0;
        //feederRate = character.GetMatchController().feederRate;
        feederRate = 0.12f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        counter += Time.deltaTime;
        if(counter >= feederRate)
        {
            counter = 0;
            if (PhotonNetwork.IsConnected && character.PV.IsMine)
            {
                character.PV.RPC("UpdateFeederScore_RPC", RpcTarget.All);
            }
            else
            {
                character.GetMatchController().UpdateFeederScore(character);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Move(character.GetInputController().leftStickInput, character.GetFeederAcceleration());
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
    public override void OnShield()
    {
        base.OnShield();
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
