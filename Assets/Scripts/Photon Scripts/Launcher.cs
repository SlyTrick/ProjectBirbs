using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;

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
        PhotonNetwork.NickName = nombre.text;
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.NickName = nombre.text;
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("menuCargando");
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MenuManager.Instance.OpenMenu("menuSeleccionModo");
            startGameButton.SetActive(true);
            //MenuManager.Instance.OpenMenu("menuSala");
        }
        else
        {
            MenuManager.Instance.OpenMenu("menuSala");
            startGameButton.SetActive(false);
        }
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in listaJugadores)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(listaJugadoresItemPrefab, listaJugadores).GetComponent<listaJugadoresItem>().SetUp(players[i]);
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
        Instantiate(listaJugadoresItemPrefab, listaJugadores).GetComponent<listaJugadoresItem>().SetUp(newPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
}
