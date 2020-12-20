using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathmatchController : ModeController
{
    public DeathmatchController(MatchController matchController) : base(matchController)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        matchController = GetComponentInParent<MatchController>();
    }
    public override void PlayerKilled(Character victim, Character killer)
    {
        if (victim.GetTeamId() == killer.GetTeamId())
            matchController.SubstractPoint(killer);
        else
            matchController.AddPoint(killer);
    }
    public override void UpdateFeederScore(Character target)
    {
        // Si por alguna razon alguien acaba en un comedero en este modo pues no suma puntos
    }
}
