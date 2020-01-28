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
        //targetRotation = new Vector3(0, 0, 21);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // make rotation more smoothly

        if (uiManager.getLock())
        {
            // if game lock, stop rotate
            return;
        }

        if(targetRotation.x < 0)
        {
            if(Mathf.Abs(targetRotation.x - currentRotation.x) > 3) // avoid shaking
            {
                // x-coordinate minus 1 per frame
                currentRotation = currentRotation - new Vector3(1, 0, 0);
                transform.eulerAngles = currentRotation;
            }
        }
        else if(targetRotation.x == 0)
        {
            if (Mathf.Abs(targetRotation.x - currentRotation.x) > 1.5)
            {
                if(currentRotation.x > 0)
                {
                    currentRotation = currentRotation - new Vector3(1, 0, 0);
                    transform.eulerAngles = currentRotation;
                }
                else
                {
                    currentRotation = currentRotation + new Vector3(1, 0, 0);
                    transform.eulerAngles = currentRotation;
                }
            }
        }
        else
        {
            if(Mathf.Abs(targetRotation.x - currentRotation.x) > 3) // avoid shaking
            {
                // x-coordinate plus 1 per frame
                currentRotation = currentRotation + new Vector3(1, 0, 0);
                transform.eulerAngles = currentRotation;
            }
        }

        if (targetRotation.z < 0)
        {
            if (Mathf.Abs(targetRotation.z - currentRotation.z) > 3)
            {
                currentRotation = currentRotation - new Vector3(0, 0, 1);
                transform.eulerAngles = currentRotation;
            }
        }
        else if (targetRotation.z == 0)
        {
            //Debug.Log(targetRotation.z);
            if (Mathf.Abs(targetRotation.z - currentRotation.z) > 1.5)
            {
                //Debug.Log(currentRotation.z);
                if (currentRotation.z > 0)
                {
                    currentRotation = currentRotation - new Vector3(0, 0, 1);
                    transform.eulerAngles = currentRotation;
                }
                else
                {
                    currentRotation = currentRotation + new Vector3(0, 0, 1);
                    transform.eulerAngles = currentRotation;
                }
            }
        }
        else
        {
            if (Mathf.Abs(targetRotation.z - currentRotation.z) > 3)
            {
                currentRotation = currentRotation + new Vector3(0, 0, 1);
                transform.eulerAngles = currentRotation;
            }
        }

        uiManager.updateRotationText(currentRotation.x,currentRotation.y,currentRotation.z);
        //transform.eulerAngles = targetRotation;
    }

    public void setRotationXAxis(float radian)
    {
        float angle = radian * 180 / Mathf.PI;
        
        // make sure the rotation angle not too big
        if(angle >= 23)
        {
            angle = 23;
        }
        else if(angle < -23)
        {
            angle = -23;
        }
        targetRotation.x = angle;
    }

    public void setRotationZAxis(float radian)
    {
        float angle = radian * 180 / Mathf.PI;

        if(angle >= 23)
        {
            angle = 23;
        }
        else if(angle <= -23)
        {
            angle = -23;
        }
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
