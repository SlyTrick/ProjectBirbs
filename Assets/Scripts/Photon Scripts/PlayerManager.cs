using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    [HideInInspector] public string[] pajaros = new string[5] { "Pigeon", "Duck", "Dori", "Kiwi", "RocketBirb" };

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }


    void Start()
    {
        if (PV.IsMine)
        {
            CrearControlador();
        }
    }

    void CrearControlador()
    {
        object indexPajaro;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("indexPajaro", out indexPajaro);
        PhotonNetwork.Instantiate(Path.Combine("Prefabs/Characters", pajaros[(int)indexPajaro]), Vector3.zero, Quaternion.identity);
    }
}
