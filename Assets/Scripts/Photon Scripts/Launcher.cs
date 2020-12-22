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

    private Dictionary<int, GameObject> listaJugadoresItems;

    private int jugadoresEnSala;

    [SerializeField] TMP_InputField nombre;
    JugadorInfo Jugador;
    
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("menuPrincipal");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(nombre.text))
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 10000);
        }
        else
        {
            PhotonNetwork.NickName = nombre.text;
        }

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
        if (string.IsNullOrEmpty(nombre.text))
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 10000);
        }
        else
        {
            PhotonNetwork.NickName = nombre.text;
        }
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MenuManager.Instance.OpenMenu("menuSeleccionModo");
            startGameButton.SetActive(true);
            cambiarModoGameButton.SetActive(true);
            //MenuManager.Instance.OpenMenu("menuSala");
        }
        else
        {
            MenuManager.Instance.OpenMenu("menuSala");
            startGameButton.SetActive(false);
            cambiarModoGameButton.SetActive(false);
        }
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in listaJugadores)
        {
            Destroy(child.gameObject);
        }

        if(listaJugadoresItems == null)
        {
            listaJugadoresItems = new Dictionary<int, GameObject>();
        }

        foreach (Player p in players)
        {
            GameObject entry = Instantiate(listaJugadoresItemPrefab);
            entry.transform.SetParent(listaJugadores);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<listaJugadoresItem>().SetUp(p);

            object pajaroActivo;
            if(p.CustomProperties.TryGetValue("indexPajaro", out pajaroActivo))
            {
                entry.GetComponent<listaJugadoresItem>().ActualizarPajaro((int)pajaroActivo);
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
        
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("menuPrincipal");
        foreach(GameObject entry in listaJugadoresItems.Values)
        {
            Destroy(entry.gameObject);
        }

        listaJugadoresItems.Clear();
        listaJugadoresItems = null;
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
            
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(listaJugadoresItemPrefab);
        entry.transform.SetParent(listaJugadores);
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
        }

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        cambiarModoGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
}
