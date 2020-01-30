using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerfulEngine : MonoBehaviour
{
    // This object: Drone

    public InterfaceManager uiManager;
    private Rigidbody physics;
    private RotationSimulator flyPose;

    private const int FORWARDS = 0;
    private const int BACKWARDS = 1;
    private const int LEFT = 2;
    private const int RIGHT = 3;

    private float propellerAcce; // accelaration of propeller
    private float mass;

    // Start is called before the first frame update
    void Start()
    {
        physics = this.GetComponent<Rigidbody>();
        flyPose = this.GetComponent<RotationSimulator>();
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
        

        if (Input.GetKey(KeyCode.Space))
        {
            //physics.AddForce(new Vector3(0,1,0) * mass * 12.0f);
        }
    }

    private void selfDriving_PerFrame()
    {
        // self driving features!!!!!!!!!!!!!!!!!!!!!!!
    }

    private float calculateTiltedAngle()
    {
        // simulate tilted and return the value of angle
        float theta = Mathf.Atan((float)( 9.8 / propellerAcce)); // accelaration of gravity is 9.8
        //Debug.Log(theta*180/Mathf.PI);
        return theta;
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
    
}
