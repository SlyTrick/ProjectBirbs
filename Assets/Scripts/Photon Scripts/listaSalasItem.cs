using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;

public class listaSalasItem : MonoBehaviour
{
    [SerializeField] Text texto;
    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        if (_info.PlayerCount == 4)
        {
            texto.text = _info.Name + "  (" + _info.PlayerCount + "/4) Llena";
        }
        texto.text = _info.Name + "  (" + _info.PlayerCount + "/4)";
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
