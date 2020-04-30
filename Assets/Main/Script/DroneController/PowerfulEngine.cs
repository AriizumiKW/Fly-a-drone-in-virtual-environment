using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class PowerfulEngine : MonoBehaviour
{
    // This object: Drone

    public float SPEED = 20.0f;
    public InterfaceManager uiManager;
    private Rigidbody physics;
    private TiltSimulator flyPose;
    private RotationSimulator rotation;

    private const int FORWARDS = 0;
    private const int BACKWARDS = 1;
    private const int LEFT = 2;
    private const int RIGHT = 3;

    private float propellerAcce; // accelaration of propeller
    private float mass;

    private RRTDrawer demoGraph;

    // Start is called before the first frame update
    void Start()
    {
        ifIdle = true;
        physics = this.GetComponent<Rigidbody>();
        flyPose = this.GetComponent<TiltSimulator>();
        rotation = this.GetComponent<RotationSimulator>();
        propellerAcce = 5.0f; // 测试用，forceMag can be modified by users
        demoGraph = this.GetComponent<RRTDrawer>();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
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
                if (nextStep) // if nextStep=true, means it need flyDaemon to update information about flyAngle, waitTime, unitDirection
                {
                    timer = 0;
                    return;
                }
                float deltaTime = Time.deltaTime;
                timer += deltaTime;
                if (timer >= waitTime)
                {
                    timer = 0;
                    nextStep = true;
                }
                this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90 - flyAngle, transform.eulerAngles.z);
                this.transform.position += unitDirection * SPEED * deltaTime; // flying
                demoGraph.drawTrace(this.transform.position); // for testing
            }
        }
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
    }

    private float calculateTiltedAngle()
    {
        // simulate tilted and return the value of angle
        float theta = Mathf.Atan((float)(propellerAcce/9.8)); // accelaration of gravity is 9.8
        return theta;
    }

    private float flyAngle;
    private Vector3 unitDirection;
    private float waitTime;
    private float timer;
    private bool nextStep;

    private IEnumerator flyDaemon() // multi-thread
    {
        for(int i=0; i < path.Count; i++)
        {
            //this.transform.position = new Vector3(path[i+1].X(), transform.position.y, path[i+1].Z());
            float currX;
            float currZ;
            float destX;
            float destZ;
            if (i == 0)
            {
                currX = this.transform.position.x;
                currZ = this.transform.position.z;
                destX = path[0].X();
                destZ = path[0].Z();
            }
            else
            {
                currX = path[i - 1].X();
                currZ = path[i - 1].Z();
                destX = path[i].X();
                destZ = path[i].Z();
            }
            float _angle;
            Vector3 _unitDirection;
            if(destX == currX)
            {
                if(destZ >= currZ)
                {
                    _angle = 90;
                    _unitDirection = new Vector3(0, 0, 1);
                }
                else
                {
                    _angle = -90;
                    _unitDirection = new Vector3(0, 0, -1);
                }
            }
            else if(destZ == currZ)
            {
                if(destX >= currX)
                {
                    _angle = 0;
                    _unitDirection = new Vector3(1, 0, 0);
                }
                else
                {
                    _angle = 180;
                    _unitDirection = new Vector3(-1, 0, 0);
                }
            }
            else
            {
                _unitDirection = new Vector3(destX - currX, 0, destZ - currZ);
                _unitDirection = _unitDirection.normalized; // return 3-demensional vector with mag=1
                if(destX >= currX)
                {
                    _angle = Mathf.Atan((destZ - currZ) / (destX - currX)) * 180 / Mathf.PI;
                }
                else
                {
                    _angle = Mathf.Atan((destZ - currZ) / (destX - currX)) * 180 / Mathf.PI - 180;
                }
                //Debug.Log(_angle);
                //demoGraph.drawPoint((int)destX, (int)destZ);
            }
            float _distance = Mathf.Sqrt(Mathf.Pow(destZ - currZ, 2) + Mathf.Pow(destX - currX, 2));
            float _waitTime = _distance / SPEED;

            nextStep = false;
            this.waitTime = _waitTime;
            this.unitDirection = _unitDirection;
            this.flyAngle = _angle;
            //this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90 - flyAngle, transform.eulerAngles.z);
            while (!nextStep)
            {
                //Debug.Log(waitTime);
                yield return new WaitForFixedUpdate();
            }
        }
        ifIdle = true; // only allow one flyDaemon running at the same time
        StopCoroutine("flyDaemon");
        yield break;
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

    private bool ifIdle; // if flyDaemon is running
    private List<RRTNode> path;
    public void letDroneFlyByPath(List<RRTNode> path)
    {
        if (ifIdle)
        {
            ifIdle = false;
            this.path = path;
            StartCoroutine("flyDaemon");
        }
    }

    public bool getIfIdle()
    {
        return ifIdle;
    }
}
