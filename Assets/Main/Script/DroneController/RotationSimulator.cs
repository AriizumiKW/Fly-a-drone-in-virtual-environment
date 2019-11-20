using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSimulator : MonoBehaviour
{
    // This object: Drone
    public InterfaceManager uiManager;
    private PowerfulEngine engine;

    private Vector3 targetRotation;
    private Vector3 currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        engine = this.GetComponent<PowerfulEngine>();
        currentRotation = transform.eulerAngles;
        targetRotation = new Vector3(0, 0, 21);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // make rotation more smoothly
        
        currentRotation = transform.eulerAngles;
        if(currentRotation.x < targetRotation.x)
        {
            if(targetRotation.x - currentRotation.x > 5)
            {
                transform.eulerAngles = currentRotation + new Vector3(0.2f, 0, 0);
            }
            
        }
        else if (currentRotation.x > targetRotation.x)
        {
            if (currentRotation.x - targetRotation.x > 5)
            {
                transform.eulerAngles = currentRotation - new Vector3(0.2f, 0, 0);
            }
        }
        if(currentRotation.z < targetRotation.z)
        {
            if (targetRotation.z - currentRotation.z > 5)
            {
                transform.eulerAngles = currentRotation + new Vector3(0, 0, 0.2f);
            }
        }
        else if (currentRotation.z > targetRotation.z)
        {
            if (currentRotation.z - targetRotation.z > 5)
            {
                transform.eulerAngles = currentRotation - new Vector3(0, 0, 0.2f);
            }
        }
        Debug.Log(currentRotation+"::"+targetRotation);
        
        //transform.eulerAngles = targetRotation;
    }

    public void setRotationXAxis(float radian)
    {
        float angle = radian * 180 / Mathf.PI;
        targetRotation.x = angle;
    }

    public void setRotationZAxis(float radian)
    {
        float angle = radian * 180 / Mathf.PI;
        targetRotation.z = angle;
    }
    /*
    private void updateRotation(int direction, float radian)
    {
        float angle = radian * 180 / Mathf.PI; // radian to angle
        switch (direction)
        {
            case FORWARDS:
                rotation.x = angle;
                break;

            case BACKWARDS:
                rotation.x = -angle;
                break;

            case LEFT:
                rotation.z = angle;
                break;

            case RIGHT:
                rotation.z = -angle;
                //transform.eulerAngles = rotation;
                break;

            default:
                break;
        }
    }

    private void smoothRotate()
    {
        Vector3 rotation = transform.eulerAngles;
        Debug.Log(rotation);
        if (transform.eulerAngles.x < rotation.x)
        {
            transform.eulerAngles = rotation + new Vector3(2, 0, 0);
        }
        else if (transform.eulerAngles.x > rotation.x)
        {
            transform.eulerAngles = rotation - new Vector3(2, 0, 0);
        }
        if (transform.eulerAngles.z < rotation.z)
        {
            transform.eulerAngles = rotation + new Vector3(0, 0, 2);
        }
        else if (transform.eulerAngles.z > rotation.z)
        {
            transform.eulerAngles = rotation - new Vector3(0, 0, 2);
        }

    }
    */
}
