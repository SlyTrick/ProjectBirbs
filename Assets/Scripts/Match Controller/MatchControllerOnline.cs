using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchControllerOnline : MatchController
{

    
    public override BoxCollider GetSpawnPoint(Character target)
    {
        return teamSpawns[target.GetTeamId()];
    }
    public override BoxCollider GetFeatherSpawn()
    {
        int random = Random.Range(0, featherSpawns.Count);

        return featherSpawns[random].GetComponent<BoxCollider>();
    }
    public override void AddPoints(Character target, int points)
    {
        int team = target.GetTeamId();
        teamsPoints[team] += points;
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
        
    }
    public override void SubstractPoints(Character target, int points)
    {
        int team = target.GetTeamId();
        teamsPoints[team] -= points;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
    }
    public override void PlayerKilled(Character victim, Character killer)
    {
        modeController.PlayerKilled(victim, killer);
    }
    public override void UpdateFeederScore(Character target)
    {
        modeController.UpdateFeederScore(target);
    }
    public override void AddFeather(Character target)
    {
        modeController.AddFeather(target);
    }
    public override void Start()
    {
        RoomManager roomManager = FindObjectOfType<RoomManager>();
        mode = roomManager.gamemodeIndex;
        switch (mode)
        {
            case (int)modes.DEATHMATCH:
                modeController = new DeathmatchController(this);
                break;
            case (int)modes.KING_OF_THE_FEEDER:
                feeder.SetActive(true);
                modeController = new KingOfTheFeederController(this);
                break;
            case (int)modes.FEATHER_HOARDER:
                for(int i = 0; i < featherSpawns.Count; i++)
                {
                    featherSpawns[i].SetActive(true);
                }
                modeController = new FeatherHoarderController(this);
                break;
        }
    }
}
