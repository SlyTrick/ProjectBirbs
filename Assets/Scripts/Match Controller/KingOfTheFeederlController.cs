using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheFeederController : ModeController
{
    public KingOfTheFeederController(MatchController matchController) : base(matchController)
    {

    }
    public override void Update()
    {
        base.Update();
    }
    public override void PlayerKilled(Character victim, Character killer)
    {
        base.PlayerKilled(victim, killer);
        // Asi no suma al matar en este modo
    }
    public override void UpdateFeederScore(Character target)
    {
        base.UpdateFeederScore(target);
        matchController.AddPoints(target, 1);
    }
    public override void AddFeather(Character target)
    {
        base.AddFeather(target);
        // Same
    }
}
