using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    public GameObject firstButtonSelected;
    
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        if(firstButtonSelected != null)
            EventSystem.current.SetSelectedGameObject(firstButtonSelected);

    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
