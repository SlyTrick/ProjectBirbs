using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 0) //Estamos en el menu principal
        {
            //PhotonNetwork.Instantiate(Path.Combine("Photon prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else if(scene.buildIndex == 1) //Estamos en el juego (¿Creo?)
        {
            //PhotonNetwork.Instantiate(Path.Combine("Photon prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnJoinedRoom()
    {
        //PhotonNetwork.Instantiate(Path.Combine("Photon prefabs", "JugadorInfo"), Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //PhotonNetwork.Instantiate(Path.Combine("Photon prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }

}
