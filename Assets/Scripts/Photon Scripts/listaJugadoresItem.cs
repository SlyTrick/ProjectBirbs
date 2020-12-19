using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class listaJugadoresItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text texto;
    Player player;

    public void SetUp(Player _player)
    {
        player = _player;
        texto.text = _player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
