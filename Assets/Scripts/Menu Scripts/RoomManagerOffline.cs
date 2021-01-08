using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RoomManagerOffline : MonoBehaviour
{
    public static RoomManagerOffline Instance;

    [SerializeField] public GameObject listaJugadoresPrefab;
    [SerializeField] public Transform listaJugadoresOffline;
    [SerializeField] public TMP_Text textoBotonCambiarModo;

    //[SerializeField] public PlayerInputManager PIM;

    public Dictionary<int, GameObject> jugadores;

    [HideInInspector] public string[] modosDeJuego = new string[3] { "Deathmatch", "Rey del comedero", "Acaparaplumas" };


    public int gamemodeIndex; //0 deathmatch, 1 rey del comedero, 2 acaparaplumas

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log(InputSystem.devices.Count);
        foreach(InputDevice d in InputSystem.devices)
        {
            Debug.Log(d.name);
        }
    }

    public void JoinOfflineRoom()
    {
        MenuManager.Instance.OpenMenu("menuSeleccionModoOffline");
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeGamemode(int newIndex)
    {
        gamemodeIndex = newIndex;
        textoBotonCambiarModo.text = "Cambiar Modo de Juego. (Actualmente " + modosDeJuego[gamemodeIndex] + ")";
        MenuManager.Instance.OpenMenu("menuSalaOffline");
    }

    public void AddPlayerToRoom()
    {
        int idJugador = jugadores.Count;
        GameObject entry = Instantiate(listaJugadoresPrefab, listaJugadoresOffline);
        entry.GetComponent<listaJugadoresItem>().SetUp(null);
        entry.GetComponent<listaJugadoresItem>().nombre = "Player " + idJugador;
        entry.GetComponent<listaJugadoresItem>().nombreTexto.text = "Player " + idJugador;
        entry.GetComponent<listaJugadoresItem>().ownerId = idJugador;
        jugadores.Add(idJugador, entry);
    }

    public void RemovePlayer(GameObject toRemove)
    {
        //PIM.JoinPlayer(0, 0, "Birb", InputSystem.devices[0]);
    }
    

    

}
