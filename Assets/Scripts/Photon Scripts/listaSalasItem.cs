using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;

public class listaSalasItem : MonoBehaviour
{
    [SerializeField] Text texto;
    RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        texto.text = _info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }
}
