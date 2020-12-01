using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected Character character;
    protected StateMachine stateMachine;

    protected State(Character character, StateMachine stateMachine)
    {
        this.character = character;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }
    public virtual void OnMove() { }
    public virtual void OnShoot() { }
    public virtual void OnLook() { }
    public virtual void OnDead() { }
}
