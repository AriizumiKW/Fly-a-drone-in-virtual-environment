using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerfulEngine : MonoBehaviour
{
    // This object: Drone

    public InterfaceManager uiManager;
    private Rigidbody physics;
    private TiltSimulator flyPose;

    private const int FORWARDS = 0;
    private const int BACKWARDS = 1;
    private const int LEFT = 2;
    private const int RIGHT = 3;

    private float propellerAcce; // accelaration of propeller
    private float mass;

    // Start is called before the first frame update
    void Start()
    {
        toUp = false;
        toDown = false;
        toLeft = false;
        toRight = false;
        physics = this.GetComponent<Rigidbody>();
        flyPose = this.GetComponent<TiltSimulator>();
        propellerAcce = 5.0f; // 测试用，forceMag can be modified by users
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(uiManager.getLock());
        if (!uiManager.getLock())
        {
            // if the game is locked, the drone state will never change
            if(uiManager.getGameMode() == InterfaceManager.MANUAL_MODE)
            {
                // manual mode, the drone can be controlled by user
                waspFly_PerFrame();
            }
            else if(uiManager.getGameMode() == InterfaceManager.SELF_DRIVING_MODE)
            {
                // in self driving mode, the drone will be "controlled" ONLY by self-driving features
                selfDriving_PerFrame();
            }
        }
        //Debug.Log(frontRefSubstance.position);
    }
    private void waspFly_PerFrame()
    {
        // manual mode. This function invoked once pre frame
        if (Input.GetKey(KeyCode.W))
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(0, 0, 1) * propellerAcce * mass * cos);
            flyPose.setRotationXAxis(cos);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(0, 0, -1) * propellerAcce * mass * cos);
            flyPose.setRotationXAxis(-cos);
        }
        else
        {
            // recover rotation
            flyPose.setRotationXAxis(0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(-1, 0, 0) * propellerAcce * mass * cos);
            flyPose.setRotationZAxis(cos);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(1, 0, 0) * propellerAcce * mass * cos);
            flyPose.setRotationZAxis(-cos);
        }
        else
        {
            // recover rotation
            flyPose.setRotationZAxis(0);
        }
        /*
        if (Input.GetKey(KeyCode.Space))
        {
            //physics.AddForce(new Vector3(0,1,0) * mass * 12.0f);
        }
        */
    }

    private bool toUp;
    private bool toDown;
    private bool toLeft;
    private bool toRight;

    private void selfDriving_PerFrame()
    {
        // self driving features
        if (toUp)
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(0, 0, 1) * propellerAcce * mass * cos);
            flyPose.setRotationXAxis(cos);
        }
        else if (toDown)
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(0, 0, -1) * propellerAcce * mass * cos);
            flyPose.setRotationXAxis(-cos);
        }
        else
        {
            // recover rotation
            flyPose.setRotationXAxis(0);
        }

        if (toLeft)
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(-1, 0, 0) * propellerAcce * mass * cos);
            flyPose.setRotationZAxis(cos);
        }
        else if (toRight)
        {
            float cos = Mathf.Cos(calculateTiltedAngle());
            physics.AddForce(new Vector3(1, 0, 0) * propellerAcce * mass * cos);
            flyPose.setRotationZAxis(-cos);
        }
        else
        {
            // recover rotation
            flyPose.setRotationZAxis(0);
        }
    }

    private float calculateTiltedAngle()
    {
        // simulate tilted and return the value of angle
        float theta = Mathf.Atan((float)( 9.8 / propellerAcce)); // accelaration of gravity is 9.8
        //Debug.Log(theta*180/Mathf.PI);
        return theta;
    }

    private IEnumerator stateToFalseThread(int whichState, float time)
    {
        yield return new WaitForSeconds(time); // wait for time seconds
        switch (whichState)
        {
            case FORWARDS:
                toUp = false;
                break;
            case BACKWARDS:
                toDown = false;
                break;
            case LEFT:
                toLeft = false;
                break;
            case RIGHT:
                toRight = false;
                break;
            default:
                Debug.LogError("PowerfulEngine: direction_not_found_error");
                break;
        }
    }

    public void resetDrone(float mass)
    {
        physics.mass = mass;
        this.mass = mass;
    }

    public void setAccelaration(float acce)
    {
        this.propellerAcce = acce;
    }

    public float getAccelaration()
    {
        return this.propellerAcce;
    }

    public void goUp(float time)
    {
        this.toUp = true;
        IEnumerator instance = stateToFalseThread(FORWARDS, time);
        StartCoroutine(instance);
    }

    public void goDown(float time)
    {
        this.toDown = true;
        IEnumerator instance = stateToFalseThread(BACKWARDS, time);
        StartCoroutine(instance);
    }

    public void goLeft(float time)
    {
        this.toLeft = true;
        IEnumerator instance = stateToFalseThread(LEFT, time);
        StartCoroutine(instance);
    }

    public void goRight(float time)
    {
        this.toRight = true;
        IEnumerator instance = stateToFalseThread(RIGHT, time);
        StartCoroutine(instance);
    }
}
