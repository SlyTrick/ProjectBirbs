﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class RoomManagerOffline : MonoBehaviour
{
    public static RoomManagerOffline Instance;

    [SerializeField] public GameObject[] pajarosPrefabs;
    [SerializeField] public GameObject listaJugadoresPrefab;
    [SerializeField] public Transform listaJugadoresOffline;
    [SerializeField] public TMP_Text textoBotonCambiarModo;
    public GameObject tituloResultados;
    public GameObject listaJugadoresResultsWinners;
    public GameObject listaJugadoresResultsLosers;
    public Launcher launcher;

    public PlayerInputManager PIM;

    public Dictionary<int, GameObject> jugadoresSala;
    public Dictionary<int, int[]> jugadoresInfo; //la posicion 0 del array es pajaroIndex, la 1 el teamId, la Key es el OwnerId
    public GameObject jugadorEntrenamiento;
    public int pajaroIndexEntrenamiento;

    [HideInInspector] public string[] modosDeJuego = new string[3] { "Deathmatch", "Rey del comedero", "Acaparaplumas" };
    [HideInInspector] public string[] equiposNombres = new string[4] { "Azul", "Rojo", "Amarillo", "Verde" };


    public int gamemodeIndex; //0 deathmatch, 1 rey del comedero, 2 acaparaplumas
    public int puntuacionFinal;
    public int teamIdGanador;
    public bool partidaOfflineTerminada = false;
    public bool salirSinTerminar = false;
    public bool doOnce = false;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) //Estamos en el juego Offline
        {
            if (doOnce)
            {
                doOnce = false;
                PIM = FindObjectOfType<PlayerInputManager>();
                foreach (KeyValuePair<int, int[]> j in jugadoresInfo)
                {
                    if (j.Key == 1)
                    {
                        PIM.playerPrefab = pajarosPrefabs[j.Value[0]];
                        InputDevice[] tecladoYRaton = new InputDevice[2] { InputSystem.devices[0], InputSystem.devices[1] };
                        PIM.JoinPlayer(-1, -1, "Keyboard and Mouse", tecladoYRaton);
                    }
                    else
                    {
                        PIM.playerPrefab = pajarosPrefabs[j.Value[0]];
                        PIM.JoinPlayer(-1, -1, "Controller", InputSystem.devices[j.Key]);
                    }
                }
            }
        }
        else if (scene.buildIndex == 0) //Estamos en el menu principal
        {
            launcher = FindObjectOfType<Launcher>();
            listaJugadoresOffline = launcher.listaJugadoresOffline;
            textoBotonCambiarModo = launcher.textoBotonCambiarModoOffline;
            jugadoresSala = null;
            if (!PhotonNetwork.IsConnected && partidaOfflineTerminada)
            {
                partidaOfflineTerminada = false;
                textoBotonCambiarModo.text = "Cambiar Modo de Juego. (Actualmente " + modosDeJuego[gamemodeIndex] + ")";
                MenuManager.Instance.OpenMenu("menuResultados");
                GoToResultsRoom();
            }
            if (salirSinTerminar)
            {
                MenuManager.Instance.OpenMenu("menuPrincipal");
            }
            
        }
        else if(scene.buildIndex == 3) //Estamos en el entrenamiento
        {
            if (doOnce)
            {
                doOnce = false;
                Debug.Log("Creo un jugador para el entrenamiento");
                PIM = FindObjectOfType<PlayerInputManager>();
                PIM.playerPrefab = pajarosPrefabs[pajaroIndexEntrenamiento];
                InputDevice[] tecladoYRaton = new InputDevice[2] { InputSystem.devices[0], InputSystem.devices[1] };
                PIM.JoinPlayer(-1, -1, "Keyboard and Mouse", tecladoYRaton);
            }
        }
    }

    public void JoinOfflineRoom()
    {
        MenuManager.Instance.OpenMenu("menuSeleccionModoOffline");
    }

    public void ChangeGamemode(int newIndex)
    {
        gamemodeIndex = newIndex;
        textoBotonCambiarModo.text = "Cambiar Modo de Juego. (Actualmente " + modosDeJuego[gamemodeIndex] + ")";
        MenuManager.Instance.OpenMenu("menuSalaOffline");
    }

    public void StartGame()
    {
        if (CheckEquipos())
        {
            jugadoresInfo = new Dictionary<int, int[]>();
            foreach (KeyValuePair<int, GameObject> j in jugadoresSala)
            {
                int[] entry = new int[] { j.Value.GetComponent<listaJugadoresItem>().pajaroIndex, j.Value.GetComponent<listaJugadoresItem>().teamId };
                jugadoresInfo.Add(j.Key, entry);
            }
            partidaOfflineTerminada = false;
            salirSinTerminar = false;
            doOnce = true;
            SceneManager.LoadScene(1);
        }
    }

    public void AddPlayerToRoom()
    {
        if(jugadoresSala == null)
        {
            jugadoresSala = new Dictionary<int, GameObject>();
        }
        if (jugadoresSala.Count >= InputSystem.devices.Count-1)
            return;

        int idJugador = jugadoresSala.Count + 1;
        GameObject entry = Instantiate(listaJugadoresPrefab, listaJugadoresOffline);
        entry.GetComponent<listaJugadoresItem>().SetUp(null);
        entry.GetComponent<listaJugadoresItem>().nombre = "Player " + idJugador;
        entry.GetComponent<listaJugadoresItem>().nombreTexto.text = "Player " + idJugador;
        entry.GetComponent<listaJugadoresItem>().ownerId = idJugador;
        jugadoresSala.Add(idJugador, entry);
    }

    public void RemovePlayer(int ownerId)
    {
        GameObject.Destroy(jugadoresSala[ownerId]);
        jugadoresSala.Remove(ownerId);
        Dictionary<int, GameObject> newDictionary = new Dictionary<int, GameObject>();
        foreach(KeyValuePair<int, GameObject> j in jugadoresSala)
        {
            int idJugador = newDictionary.Count + 1;
            j.Value.GetComponent<listaJugadoresItem>().ownerId = idJugador;
            j.Value.GetComponent<listaJugadoresItem>().nombre = "Player " + idJugador;
            j.Value.GetComponent<listaJugadoresItem>().nombreTexto.text = "Player " + idJugador;
            newDictionary.Add(idJugador, j.Value);
        }
        jugadoresSala.Clear();
        jugadoresSala = newDictionary;
    }
    
    public bool CheckEquipos()
    {
        int cantEquipo1 = 0, cantEquipo2 = 0, cantEquipo3 = 0, cantEquipo4 = 0;
        if (jugadoresSala == null)
        {
            return false;
        }
        foreach (KeyValuePair<int, GameObject> p in jugadoresSala)
        {
            int teamId = p.Value.GetComponent<listaJugadoresItem>().teamId;
            if (teamId == -1)
            {
                return false;
            }
            else
            {
                switch (teamId)
                {
                    case 0:
                        cantEquipo1++;
                        if (cantEquipo1 == jugadoresSala.Count)
                        {
                            return false;
                        }
                        break;
                    case 1:
                        cantEquipo2++;
                        if (cantEquipo2 == jugadoresSala.Count)
                        {
                            return false;
                        }
                        break;
                    case 2:
                        cantEquipo3++;
                        if (cantEquipo3 == jugadoresSala.Count)
                        {
                            return false;
                        }
                        break;
                    case 3:
                        cantEquipo4++;
                        if (cantEquipo4 == jugadoresSala.Count)
                        {
                            return false;
                        }
                        break;
                }
            }
        }
        return true;
    }

    public void TerminarPartida(int puntuacion, int teamIndexGanador)
    {
        partidaOfflineTerminada = true;
        puntuacionFinal = puntuacion;
        teamIdGanador = teamIndexGanador;
        SceneManager.LoadScene(0);
    }

    public void GoToResultsRoom()
    {
        tituloResultados = GameObject.FindGameObjectWithTag("tituloResultados");
        listaJugadoresResultsWinners = GameObject.FindGameObjectWithTag("listaJugadoresResultados");
        listaJugadoresResultsLosers = GameObject.FindGameObjectWithTag("listaJugadoresResultadosLosers");
        tituloResultados.GetComponent<TMP_Text>().text = "Ha ganado el equipo: " + equiposNombres[teamIdGanador] + ". Con " + puntuacionFinal + " puntos";
        foreach (Transform child in listaJugadoresResultsWinners.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in listaJugadoresResultsLosers.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (KeyValuePair<int, int[]> j in jugadoresInfo)
        {
            if(j.Value[1] == teamIdGanador)
            {
                Instantiate(listaJugadoresPrefab, listaJugadoresResultsWinners.transform).GetComponent<listaJugadoresItem>().
                            SetUpResultsRoom("Jugador " + j.Key, j.Value[0], true, j.Value[1]);
            }
            else
            {
                Instantiate(listaJugadoresPrefab, listaJugadoresResultsLosers.transform).GetComponent<listaJugadoresItem>().
                            SetUpResultsRoom("Jugador " + j.Key, j.Value[0], false, j.Value[1]);
            }
        }
        StartCoroutine(timerResults());
    }

    public IEnumerator timerResults()
    {
        yield return new WaitForSeconds(10);
        MenuManager.Instance.OpenMenu("menuSalaOffline");
    }

    public void StorePlayerEntrenamiento(GameObject jugador)
    {
        jugadorEntrenamiento = jugador;
        gamemodeIndex = 3;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void EmpezarEntrenamiento()
    {
        doOnce = true;
        pajaroIndexEntrenamiento = jugadorEntrenamiento.GetComponent<listaJugadoresItem>().pajaroIndex;
        SceneManager.LoadScene(3); //Nos vamos a la escena de entrenamiento
    }

    public void SalirPartida()
    {
        salirSinTerminar = true;
        SceneManager.LoadScene(0); //Al menu principal
    }

}
