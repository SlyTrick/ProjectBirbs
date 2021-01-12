using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShootingState : State
{

    public ShootingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        if (PhotonNetwork.IsConnected && character.PV.IsMine)
        {
            if (character.indiceBala == 0 || character.indiceBala == 3)
            {
                character.PV.RPC("startLoopableSound_RPC", RpcTarget.All);
            }
            else if(character.indiceBala == 2)
            {
                character.PV.RPC("startFlameThrower_RPC", RpcTarget.All);
            }
        }
        else if (!PhotonNetwork.IsConnected)
        {
            if (character.indiceBala == 0 || character.indiceBala == 3)
            {
                character.sLoopableShot.Play();
            }
            else if (character.indiceBala == 2)
            {
                character.sFlamethrower[0].Play();
                character.sFlamethrower[1].PlayDelayed(character.sFlamethrower[0].clip.length);
            }
        }
        
        
    }

    public override void Exit()
    {
        base.Exit();
        if (PhotonNetwork.IsConnected && character.PV.IsMine)
        {
            if (character.indiceBala == 0 || character.indiceBala == 3)
            {
                character.PV.RPC("stopLoopableSound_RPC", RpcTarget.All);
            }
            else if (character.indiceBala == 2)
            {
                character.PV.RPC("stopFlameThrower_RPC", RpcTarget.All);
            }
        }
        else if(!PhotonNetwork.IsConnected)
        {
            if (character.indiceBala == 0 || character.indiceBala == 3)
            {
                character.sLoopableShot.Stop();
            }
            else if (character.indiceBala == 2)
            {
                character.sFlamethrower[1].Stop();
                character.sFlamethrower[2].Play();
            }
        }
        
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
        character.Shoot();
    }

    public override void OnMove()
    {
        base.OnMove();
    }

    public override void OnShoot()
    {
        base.OnShoot();
        if (!character.GetInputController().shootInput)
            stateMachine.ChangeState(character.groundedState);
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
