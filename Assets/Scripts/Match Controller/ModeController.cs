using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController
{
    protected MatchController matchController;
    protected ModeController(MatchController matchController)
    {
        this.matchController = matchController;
    }

    public virtual void Update() { }
    public virtual void PlayerKilled(Character victim, Character killer) { }
    public virtual void UpdateFeederScore(Character target) { }
    public virtual void AddFeather(Character target) { }
}
