﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Hastable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class listaJugadoresItem : MonoBehaviour
{
    [SerializeField] TMP_Text nombreTexto;
    [SerializeField] TMP_Text pajaroTexto;
    [SerializeField] GameObject botAnteriorPajaro;
    [SerializeField] GameObject botSiguientePajaro;
    [SerializeField] GameObject botAnteriorTeam;
    [SerializeField] GameObject botSiguienteTeam;
    [SerializeField] Image imagenPajaro;
    [HideInInspector] Player player;
    [SerializeField] Sprite[] sprites;
    

    [HideInInspector] public string nombre;
    public int ownerId;

    public int gamemodeIndex; //0: deathmatch, 1: rey del comedero, 2: acaparaplumas
    [HideInInspector] public string[] pajaros = new string[5] { "Pigeon", "Duck", "Dori", "Kiwi", "RocketBirb" };
    [HideInInspector] public int pajaroIndex;
    [HideInInspector] public string pajaroActivo;

    [HideInInspector] public string indiceHashtable = "indexPajaro";
    [HideInInspector] public string indiceModoHastable = "indiceModo";


    public void SetUp(Player _player)
    {
        ownerId = _player.ActorNumber;
        player = _player;
        nombreTexto.text = _player.NickName;
        nombre = _player.NickName;
        pajaroIndex = 0;
        gamemodeIndex = 0;

        object pajaroActivo;
        if (_player.CustomProperties.TryGetValue("indexPajaro", out pajaroActivo))
        {
            pajaroIndex = (int)pajaroActivo;
            ActualizarPajaro((int)pajaroActivo);
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerId)
            {
                Hastable hash = new Hastable() { { indiceHashtable, pajaroIndex } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
            ActualizarPajaro(pajaroIndex);
        }

        object indiceModo;
        if (_player.CustomProperties.TryGetValue("indiceModo", out indiceModo))
        {
            CambiarModoDeJuego((int)indiceModo);
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            botAnteriorPajaro.SetActive(false);
            botSiguientePajaro.SetActive(false);
            botAnteriorTeam.SetActive(false);
            botSiguienteTeam.SetActive(false);
        }
    }

    public void CambiarPajaroSiguiente()
    {
        if (pajaroIndex == 4)
        {
            pajaroIndex = 0;
        }
        else
        {
            pajaroIndex++;
        }
        pajaroActivo = pajaros[pajaroIndex];
        NuevoPajaro(pajaroIndex);
    }

    public void CambiarPajaroAnterior()
    {
        if (pajaroIndex == 0)
        {
            pajaroIndex = 4;
        }
        else
        {
            pajaroIndex--;
        }
        pajaroActivo = pajaros[pajaroIndex];
        NuevoPajaro(pajaroIndex);
    }

    public void CambiarTeamSiguiente()
    {

    }

    public void CambiarTeamAnterior()
    {

    }
    
    public void NuevoPajaro(int index)
    {
        ActualizarPajaro(index);
        Hastable hash = new Hastable() { { indiceHashtable, index }};
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void ActualizarPajaro(int index)
    {
        pajaroTexto.text = pajaros[index];
        imagenPajaro.sprite = sprites[index];
    }

    public void CambiarModoDeJuego(int indiceNuevo)
    {
        gamemodeIndex = indiceNuevo;
    }
    
}
