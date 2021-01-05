using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using ExitGames.Client.Photon;
using Hastable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomName;
    [SerializeField] Transform listaSalas;
    [SerializeField] GameObject listaSalasItemPrefab;
    [SerializeField] Transform listaJugadores;
    [SerializeField] GameObject listaJugadoresItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject cambiarModoGameButton;
    [SerializeField] TMP_Text textoBotonCambiarModo;
    [SerializeField] TMP_Text textoModoDeJuegoActual;
    RoomManager roomManager;

    [HideInInspector] public string[] modosDeJuego = new string[3] { "Deathmatch", "Rey del comedero", "Acaparaplumas"};
    private Dictionary<int, GameObject> listaJugadoresItems;

    private int jugadoresEnSala;
    public bool inMenus;
    private bool comingFromMainMenu;

    [SerializeField] TMP_InputField nombre;
    JugadorInfo Jugador;
    [SerializeField] RoomManager roomManagerLocal;
    
    
    void Awake()
    {
        Instance = this;
        inMenus = false;
        comingFromMainMenu = false;
        roomManager = FindObjectOfType<RoomManager>();
    }

    public void ConnectOnline()
    {
        MenuManager.Instance.OpenMenu("menuCargando");
        PhotonNetwork.ConnectUsingSettings();
        comingFromMainMenu = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        if (string.IsNullOrEmpty(nombre.text))
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 10000);
        }
        else
        {
            PhotonNetwork.NickName = nombre.text;
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        if (comingFromMainMenu)
        {
            comingFromMainMenu = false;
            MenuManager.Instance.OpenMenu("menuSalas");
        }
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            PhotonNetwork.CreateRoom("Sala " + Random.Range(0, 10000));
        }
        else
        {
            PhotonNetwork.CreateRoom(roomNameInputField.text);
        }
        
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public void JoinRoom(RoomInfo info)
    {
        jugadoresEnSala = info.PlayerCount;
        if(jugadoresEnSala > 3)
        {
            return;
        }
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public override void OnJoinedRoom()
    {
        SetUpRoom();
    }

    public void SetUpRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MenuManager.Instance.OpenMenu("menuSeleccionModo");
            startGameButton.SetActive(true);
            cambiarModoGameButton.SetActive(true);
            textoModoDeJuegoActual.gameObject.SetActive(false);
        }
        else
        {
            MenuManager.Instance.OpenMenu("menuSala");
            startGameButton.SetActive(false);
            cambiarModoGameButton.SetActive(false);
            textoModoDeJuegoActual.gameObject.SetActive(true);
        }

        roomName.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in listaJugadores)
        {
            Destroy(child.gameObject);
        }

        if (listaJugadoresItems == null)
        {
            listaJugadoresItems = new Dictionary<int, GameObject>();
        }

        foreach (Player p in players)
        {
            GameObject entry = Instantiate(listaJugadoresItemPrefab, listaJugadores);
            //entry.transform.SetParent(listaJugadores);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<listaJugadoresItem>().SetUp(p);

            object indiceModo;
            if (p.CustomProperties.TryGetValue("indiceModo", out indiceModo))
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    textoModoDeJuegoActual.text = "Modo de Juego actual: " + modosDeJuego[(int)indiceModo];
                }
                textoBotonCambiarModo.text = "Cambiar Modo de Juego. Actual (" + modosDeJuego[(int)indiceModo] + ")";
            }
            object teamIdCambiado;
            if (p.CustomProperties.TryGetValue("indiceTeam", out teamIdCambiado))
            {
                entry.GetComponent<listaJugadoresItem>().ActualizarTeam((int)teamIdCambiado);
            }
            listaJugadoresItems.Add(p.ActorNumber, entry);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.OpenMenu("menuError");
        errorText.text = "Fallo al crear la sala: " + message;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        inMenus = true;
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public override void OnLeftRoom()
    {
        if (inMenus)
        {
            inMenus = false;
            MenuManager.Instance.OpenMenu("menuSalas");
            foreach (GameObject entry in listaJugadoresItems.Values)
            {
                Destroy(entry.gameObject);
            }

            listaJugadoresItems.Clear();
            listaJugadoresItems = null;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in listaSalas)
        {
            Destroy(trans.gameObject);
        }
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(listaSalasItemPrefab, listaSalas).GetComponent<listaSalasItem>().SetUp(roomList[i]);
            if(i >= 5)
            {
                break;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(listaJugadoresItemPrefab, listaJugadores);
        //entry.transform.SetParent(listaJugadores);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<listaJugadoresItem>().SetUp(newPlayer);

        object pajaroActivo;
        if (newPlayer.CustomProperties.TryGetValue("indexPajaro", out pajaroActivo))
        {
            entry.GetComponent<listaJugadoresItem>().ActualizarPajaro((int)pajaroActivo);
        }

        listaJugadoresItems.Add(newPlayer.ActorNumber, entry);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (listaJugadoresItems[otherPlayer.ActorNumber].gameObject == null)
            return;
        Destroy(listaJugadoresItems[otherPlayer.ActorNumber].gameObject);
        listaJugadoresItems.Remove(otherPlayer.ActorNumber);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(listaJugadoresItems == null)
        {
            listaJugadoresItems = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if(listaJugadoresItems.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object indicePajaro;
            if(changedProps.TryGetValue("indexPajaro", out indicePajaro))
            {
                entry.GetComponent<listaJugadoresItem>().ActualizarPajaro((int)indicePajaro);
            }

            object indiceModo;
            if(changedProps.TryGetValue("indiceModo", out indiceModo))
            {
                roomManager.ChangeGamemode((int)indiceModo);
                if (!PhotonNetwork.IsMasterClient)
                {
                    textoModoDeJuegoActual.text = "Modo de Juego actual: " + modosDeJuego[(int)indiceModo];
                }
                textoBotonCambiarModo.text = "Cambiar Modo de Juego. Actual (" + modosDeJuego[(int)indiceModo] + ")";
                entry.GetComponent<listaJugadoresItem>().CambiarModoDeJuego((int)indiceModo);
            }

            object teamIdCambiado;
            if(changedProps.TryGetValue("indiceTeam", out teamIdCambiado))
            {
                entry.GetComponent<listaJugadoresItem>().ActualizarTeam((int)teamIdCambiado);
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        cambiarModoGameButton.SetActive(PhotonNetwork.IsMasterClient);
        textoModoDeJuegoActual.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
    }

    public void CambiarModoDeJuego(int indiceModo)
    {
        textoBotonCambiarModo.text = "Cambiar Modo de Juego. Actual (" + modosDeJuego[indiceModo] + ")";
        Hastable hash = new Hastable() { { "indiceModo", indiceModo } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        MenuManager.Instance.OpenMenu("menuSala");
    }

    public void StartGame()
    {
        if (CheckTeams())
        {
            Debug.Log("All good, comenzando la partida");
            roomManagerLocal.players = PhotonNetwork.PlayerList;
            PhotonNetwork.LoadLevel(2);
        }
        else
        {
            Debug.Log("O un jugador no tiene equipo, o solo hay un equipo");
        }
    }

    public void VolverAlMenuPrincipal()
    {
        PhotonNetwork.Disconnect();
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        MenuManager.Instance.OpenMenu("menuPrincipal");
    }

    public bool CheckTeams()
    {
        Player[] players = PhotonNetwork.PlayerList;
        int cantEquipo1 = 0, cantEquipo2 = 0, cantEquipo3 = 0, cantEquipo4 = 0;
        foreach(Player p in players)
        {
            object teamId;
            if (p.CustomProperties.TryGetValue("indiceTeam", out teamId))
            {
                if((int)teamId == -1)
                {
                    return false;
                }
                else
                {
                    switch ((int)teamId)
                    {
                        case 0:
                            cantEquipo1++;
                            if(cantEquipo1 == players.Length)
                            {
                                return false;
                            }
                            break;
                        case 1:
                            cantEquipo2++;
                            if (cantEquipo2 == players.Length)
                            {
                                return false;
                            }
                            break;
                        case 2:
                            cantEquipo3++;
                            if (cantEquipo3 == players.Length)
                            {
                                return false;
                            }
                            break;
                        case 3:
                            cantEquipo4++;
                            if (cantEquipo4 == players.Length)
                            {
                                return false;
                            }
                            break;
                    }
                    
                }
            }
            else
            {
                Debug.Log("No se ha encontrado el teamId en las custom properties");
                return false;
            }
        }
        return true;
    }
}
