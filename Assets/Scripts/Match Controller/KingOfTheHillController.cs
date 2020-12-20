using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheFeederController : ModeController
{
    public KingOfTheFeederController(MatchController matchController) : base(matchController)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        matchController = GetComponentInParent<MatchController>();
    }
    public override void PlayerKilled(Character victim, Character killer)
    {
        // Asi no suma al matar en este modo
    }
    public override void UpdateFeederScore(Character target)
    {
        matchController.AddPoint(target);
    }
}
