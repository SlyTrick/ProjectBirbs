using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileCharacter : MonoBehaviour
{
    float horizontalMove = 0f;
    float verticalMove = 0f;
    public float speed = 5f;
    private Vector3 joystickRotation;
    private float characterRotation;

    private Vector2 leftTouchPos, rightTouchPos;
    private int leftTouchID, rightTouchID;

    Rigidbody rigidbody;


    [SerializeField] private GameObject character;
    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private Joystick rotateJoystick;
    [SerializeField] private Button mode;
    [SerializeField] private Sprite[] modeSprite;
    private Character characterScript;
    private bool shoot;
    private bool activeShield;
    private int currentMode;
    private bool stickState;

    private void Awake()
    {
        //if (SystemInfo.deviceType != DeviceType.Handheld)
        /*if(!Application.isMobilePlatform)
        {
            moveJoystick.gameObject.SetActive(false);
            rotateJoystick.gameObject.SetActive(false);
            mode.gameObject.SetActive(false);
            GetComponent<MobileCharacter>().enabled = false;
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = character.GetComponent<Rigidbody>();
        characterScript = character.GetComponent<Character>();
        shoot = true;
        currentMode = 1;
        stickState = false;
    }

    // Update is called once per frame
    void Update()
    {
       
        verticalMove = moveJoystick.Vertical * speed;
        horizontalMove = moveJoystick.Horizontal * speed;

        joystickRotation = new Vector3(rotateJoystick.Horizontal, 0f, rotateJoystick.Vertical);


        character.transform.LookAt(character.transform.position + joystickRotation);
        character.transform.position += new Vector3(horizontalMove, 0, verticalMove) * Time.deltaTime;
        if(!stickState && rotateJoystick.Horizontal != 0.0f || rotateJoystick.Vertical != 0.0f){
            if (shoot)
            {
                Debug.Log("Disparando joystick");
                characterScript.GetInputController().shootInput = true;
                characterScript.movementSM.CurrentState.OnShoot();
            }
            else
            {
                Debug.Log("Escudo joystick");
                characterScript.GetInputController().shieldInput = true;
                characterScript.movementSM.CurrentState.OnShield();
            }
            stickState = true;
        }
        else if(stickState)
        {
            if (shoot)
            {
                characterScript.GetInputController().shootInput = false;
                characterScript.movementSM.CurrentState.OnShoot();
            }
            else
            {
                characterScript.GetInputController().shieldInput = false;
                characterScript.movementSM.CurrentState.OnShield();
            }
            stickState = false;
        }
        /*else if(activeShield)
        {
            characterScript.RemoveShield();

        }*/


    }

    public void changeMode()
    {
        switch (currentMode)
        {
            case 0:
                currentMode = 1;
                break;

            case 1:
                currentMode = 0;
                break;
        }
        mode.GetComponent<Image>().sprite = modeSprite[currentMode];
        shoot = !shoot;
        Debug.Log("Modo: " + shoot);
    }
}
