using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    public GameObject tituloResultados;
    [SerializeField] public GameObject listaJugadoresItemPrefab;
    public GameObject listaJugadoresResultsWinners;
    public GameObject listaJugadoresResultsLosers;

    [HideInInspector] public string[] pajaros = new string[5] { "Pigeon", "Duck", "Dori", "Kiwi", "RocketBirb" };
    [HideInInspector] public string[] equiposNombres = new string[4] { "Azul", "Rojo", "Amarillo", "Verde" };
    public Player[] players;

    public MatchController matchController;
    private Scene actualScene;

    public int gamemodeIndex;
    private bool hostLeft;
    private float timerAmount = 5;
    private bool gameEnded = false;
    private bool tirarDelCable = false;

    GameObject myCharacter;
    GameObject myMatchController;

    public int puntosFinales;
    public int equipoGanador;

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
            players = PhotonNetwork.PlayerList;
            if (PhotonNetwork.IsMasterClient)
            {
                myMatchController = PhotonNetwork.Instantiate(Path.Combine("Prefabs/Match Elements", "Match Controller"), Vector3.zero, Quaternion.identity);
            }
            CreateCharacter();
        }else if(scene.buildIndex == 0) //Estamos en el menu principal
        {
            if (PhotonNetwork.IsConnected)
            {
                if (hostLeft)
                {
                    MenuManager.Instance.OpenMenu("menuHostDesconectado");
                    PhotonNetwork.LeaveRoom();
                    StartCoroutine(HostLeftTimer());
                }
                if (gameEnded)
                {
                    gameEnded = false;
                    MenuManager.Instance.OpenMenu("menuResultados");
                    GoToResultsRoom();
                    //FindObjectOfType<Launcher>().SetUpRoom();
                }
                if (tirarDelCable)
                {
                    tirarDelCable = false;
                    FindObjectOfType<Launcher>().VolverAlMenuPrincipal();
                }
                
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
        puntosFinales = puntos;
        equipoGanador = equipoGanadorIndex;
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

    public int FindTeamIdByPlayer(Player p)
    {
        foreach(Player j in players)
        {
            if(p.ActorNumber == j.ActorNumber)
            {
                object teamId;
                if(j.CustomProperties.TryGetValue("indiceTeam", out teamId))
                {
                    return (int)teamId;
                }
            }
        }
        return -1;
    }

    public void GoToResultsRoom()
    {
        tituloResultados = GameObject.FindGameObjectWithTag("tituloResultados");
        listaJugadoresResultsWinners = GameObject.FindGameObjectWithTag("listaJugadoresResultados");
        listaJugadoresResultsLosers = GameObject.FindGameObjectWithTag("listaJugadoresResultadosLosers");
        tituloResultados.GetComponent<TMP_Text>().text = "Ha ganado el equipo: " + equiposNombres[equipoGanador] + ". Con " + puntosFinales + " puntos";
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform child in listaJugadoresResultsWinners.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in listaJugadoresResultsLosers.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Player p in players)
        {
            object teamId;
            if(p.CustomProperties.TryGetValue("indiceTeam", out teamId))
            {
                if((int)teamId == equipoGanador)
                {
                    object pajaroActivo;
                    if(p.CustomProperties.TryGetValue("indexPajaro", out pajaroActivo))
                    {
                        Instantiate(listaJugadoresItemPrefab, listaJugadoresResultsWinners.transform).GetComponent<listaJugadoresItem>().
                            SetUpResultsRoom(p.NickName, (int)pajaroActivo, true, (int)teamId);
                    }
                }
                else
                {
                    object pajaroActivo;
                    if (p.CustomProperties.TryGetValue("indexPajaro", out pajaroActivo))
                    {
                        Instantiate(listaJugadoresItemPrefab, listaJugadoresResultsLosers.transform).GetComponent<listaJugadoresItem>().
                            SetUpResultsRoom(p.NickName, (int)pajaroActivo, false, (int)teamId);
                    }
                }
            }
        }
        StartCoroutine(ResultsTimer());
    }
    
    public IEnumerator ResultsTimer()
    {
        yield return new WaitForSeconds(10);
        FindObjectOfType<Launcher>().SetUpRoom();
    }

    public void TirarDelCable()
    {
        tirarDelCable = true;
        SceneManager.LoadScene(0); //Al menu principal
    }
}
