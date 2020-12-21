using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Hastable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class listaJugadoresItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text nombreTexto;
    [SerializeField] TMP_Text pajaroTexto;
    [SerializeField] GameObject botAnterior;
    [SerializeField] GameObject botSiguiente;
    Player player;

    public string nombre;
    public int gamemodeIndex; //1: deathmatch, 2: rey del comedero, 3: acaparaplumas

    public string[] pajaros = new string[5] { "paloma", "pato", "agapornis", "kiwi", "cuervo" };
    public int pajaroIndex;
    public string pajaroActivo;

    //PhotonView PV;

    void Awake()
    {
        //PV = GetComponent<PhotonView>();
    }

    public void SetUp(Player _player)
    {
        player = _player;
        nombreTexto.text = _player.NickName;
        nombre = _player.NickName;
        pajaroIndex = 0;
        /*if (!PV.IsMine)
        {
            botAnterior.SetActive(false);
            botSiguiente.SetActive(false);
        }*/
    }

    public void CambiarPajaroSiguiente()
    {
        if (pajaroIndex == 4)
        {
            pajaroIndex = 0;
            pajaroActivo = pajaros[pajaroIndex];
        }
        else
        {
            pajaroIndex++;
            pajaroActivo = pajaros[pajaroIndex];
        }
        NuevoPajaro(pajaroIndex);
    }

    public void CambiarPajaroAnterior()
    {
        if (pajaroIndex == 0)
        {
            pajaroIndex = 4;
            pajaroActivo = pajaros[pajaroIndex];
        }
        else
        {
            pajaroIndex--;
            pajaroActivo = pajaros[pajaroIndex];
        }
        NuevoPajaro(pajaroIndex);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    public void NuevoPajaro(int index)
    {
        pajaroTexto.text = pajaros[index];
        /*if (PV.IsMine)
        {
            Hastable hash = new Hastable();
            hash.Add("pajaroIndex", index);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }*/
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hastable changedProps)
    {
        /*if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            pajaroTexto.text = pajaros[(int)changedProps["pajarosIndex"]];
        }*/
    }
    /*
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] jugadores = info.photonView.InstantiationData;
        string _player = (string) jugadores[0];
        //player = _player;
        nombreTexto.text = _player;
        nombre = _player;
        pajaroIndex = 0;
        if (!PV.IsMine)
        {
            botAnterior.SetActive(false);
            botSiguiente.SetActive(false);
        }

        this.gameObject.transform.SetParent((Transform) jugadores[1], false);
        
    }*/
}
