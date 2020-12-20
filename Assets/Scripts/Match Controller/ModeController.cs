using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : MonoBehaviour
{
    protected MatchController matchController;
    protected ModeController(MatchController matchController)
    {
        this.matchController = matchController;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public virtual void PlayerKilled(Character victim, Character killer) { }
    public virtual void UpdateFeederScore(Character target) { }
}
