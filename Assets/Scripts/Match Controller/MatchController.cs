﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class MatchController : MonoBehaviourPunCallbacks
{
    public enum modes
    {
        DEATHMATCH,
        KING_OF_THE_FEEDER,
        FEATHER_HOARDER,
        TRAINING
    }
    public int mode;
    public int numPlayers = 4;
    public int numTeams = 4;
    public int targetScore = 10;
    public ModeController modeController;
    public List<Character> playerList = new List<Character>();
    public Dictionary<int, int> teamsPoints;

    [SerializeField] public List<BoxCollider> teamSpawnsCiudad;
    [SerializeField] public List<BoxCollider> teamSpawnsGranja;
    [SerializeField] public BoxCollider spawnEntrenamiento;
    [SerializeField] public List<GameObject> featherSpawns;
    [SerializeField] public GameObject featherPrefab;
    [SerializeField] public GameObject feederPrefab;
    [SerializeField] public GameObject cloudPrefab;
    [SerializeField] public GameObject feederPos;
    [SerializeField] public GameObject[] mapasPrefabs;
    [SerializeField] private GameObject pauseCanvas;
    public float featherRate;
    public float feederRate;
    public int numFeatherSpawns;

    [SerializeField] public PhotonView PV;
    public RoomManager roomManager;
    public RoomManagerOffline RMO;
    public int indiceMapa;

    public void AddPlayer(Character player)
    {
        playerList.Add(player);
    }

    public virtual BoxCollider GetSpawnPoint(Character target)
    {
        if(indiceMapa == 0)
        {
            return teamSpawnsCiudad[target.GetTeamId()];
        }
        else if(indiceMapa == 1)
        {
            return teamSpawnsGranja[target.GetTeamId()];
        }
        else
        {
            return spawnEntrenamiento;
        }
        
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
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].GetTeamId() == team)
                playerList[i].SetPoints(teamsPoints[team]);
        }
        if (teamsPoints[team] >= targetScore)
        {
            if (PhotonNetwork.IsConnected)
            {
                PV.RPC("EndGame_RPC", RpcTarget.All, teamsPoints[team], team);
            }
            else
            {
                foreach(Character c in playerList)
                {
                    GameObject.Destroy(c.gameObject);
                }
                RMO.TerminarPartida(teamsPoints[team], team);
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
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].PV.Owner.ActorNumber == AN)
            {
                return playerList[i];
            }
        }
        return null;
    }
    public void startDictionary()
    {
        teamsPoints = new Dictionary<int, int>();
        teamsPoints.Add(0, 0);
        teamsPoints.Add(1, 0);
        teamsPoints.Add(2, 0);
        teamsPoints.Add(3, 0);
    }
    public void RandomizeAndSpawnMapOffline()
    {
        if(RMO.gamemodeIndex == 3)
        {
            indiceMapa = 2;
        }
        else
        {
            indiceMapa = Random.Range(0, 2);
        }
        Vector3 spawnPos = new Vector3(0, 0, 0);
        Instantiate(mapasPrefabs[indiceMapa], spawnPos, Quaternion.identity);
    }

    public void Pause()
    {
        pauseCanvas.SetActive(true);
        if (PhotonNetwork.IsConnected)
        {
            foreach (Character character in playerList)
            {
                if (character.PV.IsMine)
                {
                    character.ChangePlayerInputState();
                }
            }
        }
        else
        {
            foreach (Character character in playerList)
            {
                character.ChangePlayerInputState();
            }
        }
    }

    public void Unpause()
    {
        pauseCanvas.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            foreach (Character character in playerList)
            {
                if (character.PV.IsMine)
                {
                    character.ChangePlayerInputState();
                }
            }
        }
        else
        {
            foreach (Character character in playerList)
            {
                character.ChangePlayerInputState();
            }
        }
       
    }

    public void LeaveGame()
    {
        pauseCanvas.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            roomManager.TirarDelCable();
        }
        else
        {
            foreach (Character c in playerList)
            {
                GameObject.Destroy(c.gameObject);
            }
            RMO.SalirPartida();
        }
    }

    public virtual void Start()
    {
        startDictionary();
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
                indiceMapa = Random.Range(0, 2);
                PV.RPC("SpawnMap_RPC", RpcTarget.All, indiceMapa);
            }
        }
        else
        {
            RMO = FindObjectOfType<RoomManagerOffline>();
            RandomizeAndSpawnMapOffline();
            mode = RMO.gamemodeIndex;
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
                targetScore = 10;
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
                targetScore = 100;
                modeController = new KingOfTheFeederController(this);
                break;
            case (int)modes.FEATHER_HOARDER:
                for (int i = 0; i < featherSpawns.Count; i++)
                {
                    featherSpawns[i].SetActive(true);
                }
                targetScore = 20;
                modeController = new FeatherHoarderController(this);
                break;
            case (int)modes.TRAINING:
                Object.Instantiate(feederPrefab, feederPos.transform.position, Quaternion.identity);
                for (int i = 0; i < featherSpawns.Count; i++)
                {
                    featherSpawns[i].SetActive(true);
                }
                targetScore = 999999999;
                modeController = new TrainingController(this);
                break;
        }
        numFeatherSpawns = featherSpawns.Count;
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
        Vector3 cloudDir = new Vector3(dx, 0, dz);
        Vector3 cloudPos = new Vector3(px, 0, pz);
        Instantiate(cloudPrefab, cloudPos, Quaternion.LookRotation(cloudDir));
    }

    [PunRPC]
    public void SpawnMap_RPC(int inMapa)
    {
        indiceMapa = inMapa;
        Vector3 spawnPos = new Vector3(0, 0, 0);
        Instantiate(mapasPrefabs[indiceMapa], spawnPos, Quaternion.identity);
    }
}
