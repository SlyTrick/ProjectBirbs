using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    private enum modes
    {
        DEATHMATCH,
        KING_OF_THE_FEEDER,
        CAPTURE_THE_FLAG
    }
    private int mode;
    private int numPlayers = 4;
    private int numTeams = 4;
    private ModeController modeController;
    private List<Character> playerList = new List<Character>();
    private List<int> teamsPoints = new List<int>();
    
    [SerializeField]private BoxCollider spawn1;
    [SerializeField]private BoxCollider spawn2;
    [SerializeField]private BoxCollider spawn3;
    [SerializeField]private BoxCollider spawn4;
    [SerializeField]private GameObject feeder;
    public float feederRate;

    public void AddPlayer(Character player)
    {
        playerList.Add(player);
        if(teamsPoints.Count != player.GetTeamId() + 1)
            teamsPoints.Add(0);

    }
    
    public BoxCollider GetSpawnPoint(Character target)
    {
        BoxCollider targetSpawn = null;
        switch (target.GetTeamId())
        {
            case 0:
                targetSpawn = spawn1;
                break;
            case 1:
                targetSpawn = spawn2;
                break;
            case 2:
                targetSpawn = spawn3;
                break;
            case 3:
                targetSpawn = spawn4;
                break;
        }
        return targetSpawn;
    }
    public void AddPoint(Character target)
    {
        int team = target.GetTeamId();
        teamsPoints[team]++;
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
        
    }
    public void SubstractPoint(Character target)
    {
        int team = target.GetTeamId();
        teamsPoints[team]--;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
    }
    public void PlayerKilled(Character victim, Character killer)
    {
        modeController.PlayerKilled(victim, killer);
    }
    public void UpdateFeederScore(Character target)
    {
        modeController.UpdateFeederScore(target);
    }
    void Start()
    {
        mode = (int)modes.KING_OF_THE_FEEDER;
        switch (mode)
        {
            case (int)modes.DEATHMATCH:
                modeController = new DeathmatchController(this);
                break;
            case (int)modes.KING_OF_THE_FEEDER:
                feeder.SetActive(true);
                modeController = new KingOfTheFeederController(this);
                break;
            case (int)modes.CAPTURE_THE_FLAG:

                break;
        }
    }

}
