using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class listaSalasItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI texto;
    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        bool abierta = _info.IsOpen;
        if (abierta)
        {
            texto.text = _info.Name + "  (" + _info.PlayerCount + "/4) (Esperando...)";
        }
        else
        {
            texto.text = _info.Name + "  (" + _info.PlayerCount + "/4) (Jugando)";
        }
        
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
