using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    // This gameobject: Main Camera

    public InterfaceManager uiManager; // global interface manager
    public Transform dronePosition; // drone's position

    private const float ROTATE_SPEED = 2.0f;
    private const float SCROLL_SPEED = 3.0f;
    private const int MOUSE_LEFT_BUTTON = 0;
    private const int MOUSE_RIGHT_BUTTON = 1;
    private const bool FIRST_PERSONAL_PERSPECTIVE = false;
    private const bool THIRD_PERSONAL_PERSPECTIVE = true;

    private bool isRotate; // is true when mouse right button down
    private bool perspectiveMode; // is true when third personal perspective, is false when first personal perspective

    // Start is called before the first frame update
    void Start()
    {
        perspectiveMode = THIRD_PERSONAL_PERSPECTIVE;
    }

    void Update()
    {
        if (!uiManager.getLock())
        {
            // if game is not locked, this part invoked per Update()
            if (Input.GetKeyDown(KeyCode.F))
            {
                perspectiveMode = !perspectiveMode;
                if (perspectiveMode == FIRST_PERSONAL_PERSPECTIVE)
                {
                    changeToFP();
                }else if (perspectiveMode == THIRD_PERSONAL_PERSPECTIVE)
                {
                    changeToTP();
                }
            }
            if(perspectiveMode == THIRD_PERSONAL_PERSPECTIVE)
            {
                scrollView();
                changeRotation();
                //stableCameraInTP();
            }
        }
    }
    /*
    void stableCameraInTP()
    {
        // stabilize rotation
        
        float x = transform.localEulerAngles.x;
        float y = transform.localEulerAngles.y;
        float z = transform.localEulerAngles.z;
        transform.eulerAngles = new Vector3(0, 0, 0);
        
        Debug.Log(transform.localEulerAngles+":"+transform.eulerAngles);
    }
    */
    void changeToFP()
    {
        // change to first personal perspective
        transform.localPosition = new Vector3(0, 0.7f, 1.0f);
        transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    void changeToTP()
    {
        // change to third personal perspective
        transform.localPosition = new Vector3(0, 0.8f, -3.0f);
        transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    void changeRotation()
    {
        // change camera rotation by mouse
        if (Input.GetMouseButtonDown(MOUSE_RIGHT_BUTTON))
        {
            //Debug.Log("right mouse down");
            isRotate = true;
        }
        if (Input.GetMouseButtonUp(MOUSE_RIGHT_BUTTON))
        {
            //Debug.Log("right mouse up");
            isRotate = false;
        }

        if (isRotate)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            transform.RotateAround(dronePosition.position, dronePosition.up, Input.GetAxis("Mouse X") * ROTATE_SPEED);
            transform.RotateAround(dronePosition.position, transform.right, Input.GetAxis("Mouse Y") * ROTATE_SPEED);

            //Debug.Log(transform.eulerAngles.x)
        }
    }

    void scrollView()
    {
        // manipulate mouse scrollwheel, will lead the 
        float fieldOfView = Camera.main.fieldOfView;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (fieldOfView <= 150.0f)
            {
                Camera.main.fieldOfView += SCROLL_SPEED;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (fieldOfView >= 50.0f)
            {
                Camera.main.fieldOfView -= SCROLL_SPEED;
            }
        }

        //radisSquare = (dronePosition.position - cameraPosition.position).sqrMagnitude;
    }
}
