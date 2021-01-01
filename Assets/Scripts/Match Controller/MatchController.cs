using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchController : MonoBehaviourPunCallbacks
{
    public enum modes
    {
        DEATHMATCH,
        KING_OF_THE_FEEDER,
        FEATHER_HOARDER
    }
    public int mode;
    public int numPlayers = 4;
    public int numTeams = 4;
    public ModeController modeController;
    public List<Character> playerList = new List<Character>();
    public List<int> teamsPoints = new List<int>();
    
    [SerializeField] public List<BoxCollider> teamSpawns;
    [SerializeField] public List<GameObject> featherSpawns;
    [SerializeField] public GameObject featherPrefab;
    [SerializeField] public GameObject feederPrefab;
    [SerializeField] public GameObject cloudPrefab;
    [SerializeField] public GameObject feederPos;
    public float featherRate;   
    public float feederRate;
    public int numFeatherSpawns = 3;

    [SerializeField] public PhotonView PV;
    public RoomManager roomManager;

    public int AddPlayer(Character player)
    {
        playerList.Add(player);
        int playerteamId = playerList.Count - 1;
        //if(teamsPoints.Count != player.GetTeamId() + 1)
        if (teamsPoints.Count != playerteamId + 1)
            teamsPoints.Add(0);
        return playerList.Count - 1;

    }

    public virtual BoxCollider GetSpawnPoint(Character target)
    {
        return teamSpawns[target.GetTeamId()];
    }
    public virtual BoxCollider GetFeatherSpawn()
    {
        int random = Random.Range(0, featherSpawns.Count);

        return featherSpawns[random].GetComponent<BoxCollider>();
    }
    public virtual void AddPoints(Character target, int points)
    {
        int team = target.GetTeamId();
        teamsPoints[team] += points;
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
        if(teamsPoints[team] >= 10)
        {
            if (PhotonNetwork.IsConnected)
            {
                PV.RPC("EndGame_RPC", RpcTarget.All, teamsPoints[team], team);
            }
            
        }
    }

    public virtual void SubstractPoints(Character target, int points)
    {
        int team = target.GetTeamId();
        teamsPoints[team] -= points;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
    }

    public virtual void PlayerKilled(Character victim, Character killer)
    {
        modeController.PlayerKilled(victim, killer);
    }

    public virtual void UpdateFeederScore(Character target)
    {
        modeController.UpdateFeederScore(target);
    }

    public virtual void AddFeather(Character target)
    {
        modeController.AddFeather(target);
    }

    public Character findByActorNumber(int AN)
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].PV.Owner.ActorNumber == AN)
            {
                return playerList[i];
            }
        }
        return null;
    }

    public virtual void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            roomManager = FindObjectOfType<RoomManager>();
            mode = roomManager.gamemodeIndex;
            if (PhotonNetwork.IsMasterClient)
            {
                float dirX = Random.Range(-1.0f, 1.0f);
                float dirZ = Random.Range(-1.0f, 1.0f);
                float posX = Random.Range(GetFeatherSpawn().bounds.min.x, GetFeatherSpawn().bounds.max.x);
                float posZ = Random.Range(GetFeatherSpawn().bounds.min.z, GetFeatherSpawn().bounds.max.z);
                PV.RPC("SpawnCloud_RPC", RpcTarget.All, dirX, dirZ, posX, posZ);
            }
        }
        else
        {
            mode = (int)modes.FEATHER_HOARDER;
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
        }
        switch (mode)
        {
            case (int)modes.DEATHMATCH:
                modeController = new DeathmatchController(this);
                break;
            case (int)modes.KING_OF_THE_FEEDER:
                if (PhotonNetwork.IsConnected)
                {
                    PV.RPC("SpawnFeeder_RPC", RpcTarget.All);
                }
                else
                {
                    Object.Instantiate(feederPrefab, feederPos.transform.position, Quaternion.identity);
                }
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

    public void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
        }
        modeController.Update();
    }

    #region RPCs_FeatherHoarded

    [PunRPC]
    public void SpawnFeather_RPC(float x, float z)
    {
        Vector3 spawnPos = new Vector3(x, 0, z);
        Object.Instantiate(featherPrefab, spawnPos, Quaternion.identity);
    }

    [PunRPC]
    public void SpawnLostFeather_RPC(float x, float z, float dx, float dz)
    {
        Vector3 spawnDir = new Vector3(dx, 0, dz);
        Vector3 spawnPos = new Vector3(x, 0, z);
        FeatherController feather = Object.Instantiate(featherPrefab, spawnPos, Quaternion.Euler(spawnDir)).GetComponent<FeatherController>();
        feather.rigidBody.AddForce(spawnDir * feather.acceleration * Time.fixedDeltaTime, ForceMode.Impulse);
    }
    #endregion

    #region RPCs_KingOfTheFeeder

    [PunRPC]
    public void SpawnFeeder_RPC()
    {
        Object.Instantiate(feederPrefab, feederPos.transform.position, Quaternion.identity);
    }

    #endregion

    [PunRPC]
    public void EndGame_RPC(int pointsWinner, int indexWinnerTeam)
    {
        roomManager.TerminarPartida(pointsWinner, indexWinnerTeam);
    }

    [PunRPC]
    public void SpawnCloud_RPC(float dx, float dz, float px, float pz)
    {
        Vector3 cloudPos = new Vector3(px, 0, pz);
        Vector3 cloudDir = new Vector3(dx, 0, dz);
        Instantiate(cloudPrefab, cloudPos, Quaternion.LookRotation(cloudDir));
    }
}
