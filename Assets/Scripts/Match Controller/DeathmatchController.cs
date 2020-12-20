using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathmatchController : ModeController
{
    public DeathmatchController(MatchController matchController) : base(matchController)
    {

    }

    public override void Update()
    {
        base.Update();
    }
    public override void PlayerKilled(Character victim, Character killer)
    {
        base.PlayerKilled(victim, killer);
        if (victim.GetTeamId() == killer.GetTeamId())
            matchController.SubstractPoints(killer, 1);
        else
            matchController.AddPoints(killer, 1);
    }
    public override void UpdateFeederScore(Character target)
    {
        base.UpdateFeederScore(target);
        // Si por alguna razon alguien acaba en un comedero en este modo pues no suma puntos
    }
    public override void AddFeather(Character target)
    {
        base.AddFeather(target);
        // Same
    }
}
