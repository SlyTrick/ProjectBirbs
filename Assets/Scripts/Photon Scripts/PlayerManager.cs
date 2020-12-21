using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

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
        
        //PhotonNetwork.Instantiate(Path.Combine("Photon prefabs", "JugadorInfo"), Vector3.zero, Quaternion.identity);
    }
}
