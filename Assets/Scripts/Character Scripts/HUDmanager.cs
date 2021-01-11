using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDmanager : MonoBehaviour
{

    [SerializeField] private Sprite[] scoreBackgrounds;
    [SerializeField] private GameObject scoreSprite;
    [SerializeField] private Slider slider;
    private Character characterScript;

    // Start is called before the first frame update
    void Start()
    {
        characterScript = GetComponent<Character>();
        
        slider.maxValue = characterScript.life;
    }

    private void Update()
    {
        slider.value = characterScript.life;
    }

    public void setBackground()
    {
        characterScript = GetComponent<Character>();
        scoreSprite.GetComponent<Image>().sprite = scoreBackgrounds[characterScript.teamId];
    }

}
