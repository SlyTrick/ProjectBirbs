using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Hastable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using UnityEngine.Localization.Settings;

public class listaJugadoresItem : MonoBehaviour
{
    [SerializeField] public TMP_Text nombreTexto;
    [SerializeField] TMP_Text pajaroTexto;
    [SerializeField] GameObject botAnteriorPajaro;
    [SerializeField] GameObject botSiguientePajaro;
    [SerializeField] GameObject botAnteriorTeam;
    [SerializeField] GameObject botSiguienteTeam;
    [SerializeField] GameObject botEliminar;
    [SerializeField] Image imagenPajaro;
    [SerializeField] Image marcoTeam;
    [HideInInspector] Player player;
    [SerializeField] Sprite[] spritesPajaros;
    [SerializeField] Sprite[] spritesResultsWinners;
    [SerializeField] Sprite[] spritesResultsLosers;
    [SerializeField] Image imagenResults;
    [SerializeField] Image glowTeam;
    [SerializeField] TMP_Text nombreResults;
    

    [HideInInspector] public string nombre;
    public int ownerId;
    public int teamId;

    public int gamemodeIndex; //0: deathmatch, 1: rey del comedero, 2: acaparaplumas
    [HideInInspector] public string[] pajarosSpanish = new string[5] { "Crnl. Columba", "Dr. Platyrhynski", "Dori", "Apsteryx", "Corvin" };
    [HideInInspector] public string[] pajarosEnglish = new string[5] { "Lt. Columba", "Dr. Platyrhynski", "Dori", "Apsteryx", "Corvin" };
    [HideInInspector] public Color[] coloresTeam = new Color[5] { Color.blue, Color.red, Color.yellow, Color.green, Color.gray };
    [HideInInspector] public int pajaroIndex;
    public string pajaroActivo;

    [HideInInspector] public string indiceHashtable = "indexPajaro";
    [HideInInspector] public string indiceModoHastable = "indiceModo";
    [HideInInspector] public string indiceTeamHashtable = "indiceTeam";


    public bool offline;
    public PlayerInput playerInput;

    public void SetUp(Player _player)
    {
        if(_player != null)
        {
            offline = false;
            ownerId = _player.ActorNumber;
            player = _player;
            nombreTexto.text = _player.NickName;
            nombre = _player.NickName;
            pajaroIndex = 0;
            gamemodeIndex = 0;
            botEliminar.SetActive(false);

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
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerId)
            {
                teamId = -1;
                Hastable hash = new Hastable() { { indiceTeamHashtable, teamId } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
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
        else
        {
            offline = true;
            pajaroIndex = 0;
            teamId = -1;
            ActualizarPajaro(pajaroIndex);
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
        if (LocalizationSettings.SelectedLocale.name == "Spanish (es)")
        {
            pajaroActivo = pajarosSpanish[pajaroIndex];
        }
        else
        {
            pajaroActivo = pajarosEnglish[pajaroIndex];
        }
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
        if (LocalizationSettings.SelectedLocale.name == "Spanish (es)")
        {
            pajaroActivo = pajarosSpanish[pajaroIndex];
        }
        else
        {
            pajaroActivo = pajarosEnglish[pajaroIndex];
        }
        NuevoPajaro(pajaroIndex);
    }

    public void CambiarTeamSiguiente()
    {
        if(teamId == 3 || teamId == -1)
        {
            teamId = 0;
        }
        else
        {
            teamId++;
        }
        CambiarTeam(teamId);
    }

    public void CambiarTeamAnterior()
    {
        if(teamId == 0 || teamId == -1)
        {
            teamId = 3;
        }
        else
        {
            teamId--;
        }
        CambiarTeam(teamId);
    }
    
    public void NuevoPajaro(int index)
    {
        ActualizarPajaro(index);
        Hastable hash = new Hastable() { { indiceHashtable, index }};
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void ActualizarPajaro(int index)
    {
        if (LocalizationSettings.SelectedLocale.name == "Spanish (es)")
        {
            pajaroTexto.text = pajarosSpanish[index];
        }
        else
        {
            pajaroTexto.text = pajarosEnglish[index];
        }
        imagenPajaro.sprite = spritesPajaros[index];
    }

    public void CambiarModoDeJuego(int indiceNuevo)
    {
        gamemodeIndex = indiceNuevo;
    }

    public void CambiarTeam(int newTeamId)
    {
        ActualizarTeam(newTeamId);
        Hastable hash = new Hastable() { { indiceTeamHashtable, newTeamId } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void ActualizarTeam(int newTeamId)
    {
        if(newTeamId == -1)
        {
            marcoTeam.color = coloresTeam[4];
        }
        else
        {
            marcoTeam.color = coloresTeam[newTeamId];
        }
    }

    public void SetUpResultsRoom(string nombre, int pajaroIndex, bool winner, int teamId)
    {
        botAnteriorPajaro.SetActive(false);
        botSiguientePajaro.SetActive(false);
        botAnteriorTeam.SetActive(false);
        botSiguienteTeam.SetActive(false);
        marcoTeam.gameObject.SetActive(false);
        pajaroTexto.gameObject.SetActive(false);
        botEliminar.SetActive(false);
        imagenPajaro.gameObject.SetActive(false);
        nombreTexto.gameObject.SetActive(false);
        nombreResults.gameObject.SetActive(true);
        imagenResults.gameObject.SetActive(true);
        glowTeam.gameObject.SetActive(true);
        nombreResults.text = nombre;
        glowTeam.color = coloresTeam[teamId];
        if (winner)
        {
            imagenResults.sprite = spritesResultsWinners[pajaroIndex];
        }
        else
        {
            imagenResults.sprite = spritesResultsLosers[pajaroIndex];
            this.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void EliminarJugador()
    {
        RoomManagerOffline RMO = FindObjectOfType<RoomManagerOffline>();
        RMO.RemovePlayer(ownerId);
    }
    
    public void SetUpTrainingRoom()
    {
        botAnteriorTeam.SetActive(false);
        botSiguienteTeam.SetActive(false);
        marcoTeam.gameObject.SetActive(false);
        botEliminar.SetActive(false);
        nombreTexto.gameObject.SetActive(false);
        pajaroIndex = 0;
        ActualizarPajaro(pajaroIndex);
        this.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}
