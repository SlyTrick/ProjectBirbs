using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorInfo : MonoBehaviour
{
    private string nombre;

    public string getNombre()
    {
        return nombre;
    }

    public void setNombre(string n)
    {
        nombre = n;
    }
}
