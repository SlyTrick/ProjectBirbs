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

    public int gamemodeIndex;

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

        if(scene.buildIndex == 2) //Estamos en el juego Online
        {
            PhotonNetwork.Instantiate(Path.Combine("Photon prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(Path.Combine("Prefabs/Match Elements", "Online Match Controller"), Vector3.zero, Quaternion.identity);
            }
        }
    }

    public void ChangeGamemode(int index)
    {
        gamemodeIndex = index;
    }
}
