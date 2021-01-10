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
    private Character characterScript;
    private bool shoot;
    private bool activeShield;

    private void Awake()
    {
        //if (SystemInfo.deviceType != DeviceType.Handheld)
        if(!Application.isMobilePlatform)
        {
            moveJoystick.gameObject.SetActive(false);
            rotateJoystick.gameObject.SetActive(false);
            mode.gameObject.SetActive(false);
            GetComponent<MobileCharacter>().enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = character.GetComponent<Rigidbody>();
        characterScript = character.GetComponent<Character>();
        shoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.touchCount > 0)
        {
            foreach(Touch touch in Input.touches)
            {
                if (touch.position.x > middle.transform.position.x && !rotateJoystick.gameObject.activeInHierarchy) //Derecha
                {
                    rightTouchID = touch.fingerId;
                    rotateJoystick.transform.position = new Vector3(touch.position.x, rotateJoystick.transform.position.y, touch.position.y);
                    rotateJoystick.gameObject.SetActive(true);
                }
                else if(touch.position.x <= middle.transform.position.x && !moveJoystick.gameObject.activeInHierarchy)//Izquierda
                {
                    leftTouchID = touch.fingerId;
                    moveJoystick.transform.position = new Vector3(touch.position.x, rotateJoystick.transform.position.y, touch.position.y);
                    moveJoystick.gameObject.SetActive(true);
                }
            }
        }*/
        

        verticalMove = moveJoystick.Vertical * speed;
        horizontalMove = moveJoystick.Horizontal * speed;

        joystickRotation = new Vector3(rotateJoystick.Horizontal, 0f, rotateJoystick.Vertical);


        character.transform.LookAt(character.transform.position + joystickRotation);
        character.transform.position += new Vector3(horizontalMove, 0, verticalMove) * Time.deltaTime;
        if(rotateJoystick.Horizontal != 0.0f || rotateJoystick.Vertical != 0.0f){
            if (shoot)
            {
                Debug.Log("Disparando joystick");
                characterScript.Shoot();
            }
            else if(!activeShield && !shoot)
            {
                Debug.Log("Escudo joystick");
                activeShield = true;
                characterScript.CreateShield();
            }
        }
        else if(activeShield)
        {
            characterScript.RemoveShield();
        }




        /*foreach(Touch t in Input.touches)
        {
            if(t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                if(t.fingerId == rightTouchID)
                {
                    rotateJoystick.gameObject.SetActive(false);
                }
                else if(t.fingerId == leftTouchID)
                {
                    moveJoystick.gameObject.SetActive(false);
                }
            }
        }*/
    }

    public void changeMode()
    {
        shoot = !shoot;
        Debug.Log("Modo: " + shoot);
    }
}
