using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class JugadorInfo : MonoBehaviour
{
    private string nombre;
    public int gamemodeIndex; //1: deathmatch, 2: rey del comedero, 3: acaparaplumas

    public string[] pajaros = new string[5] { "paloma", "pato", "agapornis", "kiwi", "cuervo" };
    public int pajaroIndex;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }


    public string getNombre()
    {
        return nombre;
    }

    public void setNombre(string n)
    {
        nombre = n;
    }
}
