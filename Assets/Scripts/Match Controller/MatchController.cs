using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    private enum modes
    {
        DEATHMATCH,
        KING_OF_THE_FEEDER,
        FEATHER_HOARDER
    }
    private int mode;
    private int numPlayers = 4;
    private int numTeams = 4;
    private ModeController modeController;
    private List<Character> playerList = new List<Character>();
    private List<int> teamsPoints = new List<int>();
    
    [SerializeField]private List<BoxCollider> teamSpawns;
    [SerializeField]private List<GameObject> featherSpawns;
    [SerializeField]public GameObject featherPrefab;
    [SerializeField]private GameObject feederPrefab;
    [SerializeField]private GameObject cloudPrefab;
    public float featherRate;   
    public float feederRate;
    private int numFeatherSpawns = 3;

    public void AddPlayer(Character player)
    {
        playerList.Add(player);
        if(teamsPoints.Count != player.GetTeamId() + 1)
            teamsPoints.Add(0);

    }

    public BoxCollider GetSpawnPoint(Character target)
    {
        return teamSpawns[target.GetTeamId()];
    }
    public BoxCollider GetFeatherSpawn()
    {
        int random = Random.Range(0, featherSpawns.Count);

        return featherSpawns[random].GetComponent<BoxCollider>();
    }
    public void AddPoints(Character target, int points)
    {
        int team = target.GetTeamId();
        teamsPoints[team] += points;
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
        
    }
    public void SubstractPoints(Character target, int points)
    {
        int team = target.GetTeamId();
        teamsPoints[team] -= points;
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
    public void AddFeather(Character target)
    {
        modeController.AddFeather(target);
    }
    void Start()
    {
        Vector3 cloudDir = new Vector3(
                Random.Range(-1.0f, 1.0f),
                0,
                Random.Range(-1.0f, 1.0f)
            );
        Vector3 cloudPos = new Vector3(
            Random.Range(GetFeatherSpawn().bounds.min.x, GetFeatherSpawn().bounds.max.x),
            0,
            Random.Range(GetFeatherSpawn().bounds.min.z, GetFeatherSpawn().bounds.max.z)
        );
        Instantiate(cloudPrefab, cloudPos, Quaternion.LookRotation(cloudDir));
        mode = (int)modes.FEATHER_HOARDER;
        switch (mode)
        {
            case (int)modes.DEATHMATCH:
                modeController = new DeathmatchController(this);
                break;
            case (int)modes.KING_OF_THE_FEEDER:
                feederPrefab.SetActive(true);
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
    private void Update()
    {
        modeController.Update();
    }
}
