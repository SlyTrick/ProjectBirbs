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
    [HideInInspector] public string[] pajaros = new string[5] { "Pigeon", "Duck", "Dori", "Kiwi", "RocketBirb" };

    public MatchController matchController;
    private Scene actualScene;

    public int gamemodeIndex;
    private bool hostLeft;
    private float timerAmount = 5;
    private bool gameEnded = false;

    GameObject myCharacter;
    GameObject myMatchController;

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
        //DontDestroyOnLoad(gameObject);
        actualScene = scene;
        if(scene.buildIndex == 2) //Estamos en el juego Online
        {
            if (PhotonNetwork.IsMasterClient)
            {
                myMatchController = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Match Elements", "Match Controller"), Vector3.zero, Quaternion.identity);
            }
            CreateCharacter();
        }else if(scene.buildIndex == 0) //Estamos en el menu principal
        {
            if (hostLeft)
            {
                MenuManager.Instance.OpenMenu("menuHostDesconectado");
                PhotonNetwork.LeaveRoom();
                StartCoroutine(HostLeftTimer());
            }
            if(gameEnded)
            {
                gameEnded = false;
                FindObjectOfType<Launcher>().SetUpRoom();
            }
        }
    }

    public void ChangeGamemode(int index)
    {
        gamemodeIndex = index;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient && actualScene.buildIndex == 2)
        {
            matchController = FindObjectOfType<MatchController>();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(actualScene.buildIndex == 2)
        {
            hostLeft = true;
            PhotonNetwork.Destroy(myCharacter);
            PhotonNetwork.LoadLevel(0);
        }
    }

    public void TerminarPartida(int puntos, int equipoGanadorIndex)
    {
        gameEnded = true;
        PhotonNetwork.Destroy(myCharacter);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.Destroy(myMatchController);
            PhotonNetwork.LoadLevel(0);
        }
    }

    IEnumerator HostLeftTimer()
    {
        yield return new WaitForSeconds(timerAmount);
        MenuManager.Instance.OpenMenu("menuSalas");
    }

    public void CreateCharacter()
    {
        object indexPajaro;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("indexPajaro", out indexPajaro);
        myCharacter = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Characters", pajaros[(int)indexPajaro]), Vector3.zero, Quaternion.identity);
    }
    
}
