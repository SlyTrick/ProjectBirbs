using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;
    [SerializeField] TMP_InputField nombre;
    [SerializeField] JugadorInfo jugador;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string nombre)
    {
        for(int i = 0; i<menus.Length; i++)
        {
            if(menus[i].menuName == nombre)
            {
                OpenMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if(menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void comprobarNombre()
    {
        if (string.IsNullOrEmpty(nombre.text))
        {
            //return;
        }
        //jugador.setNombre(nombre.text);
        OpenMenu("menuSalas");
    }
}
