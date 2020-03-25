using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class PowerfulEngine : MonoBehaviour
{
    // This object: Drone

    public float SPEED = 10.0f;
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
        flyLock = true;
        physics = this.GetComponent<Rigidbody>();
        flyPose = this.GetComponent<TiltSimulator>();
        rotation = this.GetComponent<RotationSimulator>();
        propellerAcce = 5.0f; // 测试用，forceMag can be modified by users
        demoGraph = this.GetComponent<RRTDrawer>();
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
    }

    private float flyAngle;
    private Vector3 unitDirection;
    private float waitTime;
    private float timer;
    private bool flyLock;
    private void selfDriving_PerFrame()
    {
        // self driving features
        if (flyLock)
        {
            return;
        }
        timer += Time.deltaTime;
        rotation.setRotatedAngle(90 - flyAngle);
        this.transform.position += unitDirection * SPEED * Time.deltaTime; // moving
        demoGraph.drawPoint((int)this.transform.position.x, (int)this.transform.position.z, Scalar.Green, 1);
        if (timer >= waitTime)
        {
            timer = 0;
            flyLock = true;
        }
    }

    private float calculateTiltedAngle()
    {
        // simulate tilted and return the value of angle
        float theta = Mathf.Atan((float)( 9.8 / propellerAcce)); // accelaration of gravity is 9.8
        //Debug.Log(theta*180/Mathf.PI);
        return theta;
    }

    private IEnumerator flyDaemon(List<RRTNode> path)
    {
        foreach(RRTNode node in path)
        {
            demoGraph.drawPoint((int)node.X(), (int)node.Z(), Scalar.Purple, 2);
        }
        for (int i = 0; i < path.Count; i++)
        {
            float currX;
            float currZ;
            float destX;
            float destZ;
            if(i != 0)
            {
                currX = path[i - 1].X();
                currZ = path[i - 1].Z();
                destX = path[i].X();
                destZ = path[i].Z();
            }
            else
            {
                currX = this.transform.position.x;
                currZ = this.transform.position.z;
                destX = path[0].X();
                destZ = path[0].Z();
            }
            float distance = Mathf.Sqrt(Mathf.Pow(destZ - currZ, 2) + Mathf.Pow(destX - currX, 2));
            if(currX == destX)
            {
                this.flyAngle = 90;
                this.unitDirection = new Vector3(0, 0, 1);
            }
            else
            {
                float radian = Mathf.Atan((destZ - currZ) / (destX - currX));
                this.flyAngle = radian * 180 / Mathf.PI;
                this.unitDirection = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
            }
            this.flyLock = false;
            this.waitTime = distance / SPEED;
            Debug.Log(distance);
            if (true) //测试
            {
                demoGraph.drawPoint((int)currX, (int)currZ);
                //demoGraph.drawPoint((int)currX + 10, (int)currZ);
                demoGraph.drawPoint((int)destX, (int)destZ);
            }
            this.timer = 0;
            while (!flyLock)
            {
                yield return new WaitForSeconds(0.02f);
            }
        }
        ifIdle = true;
        StopCoroutine("flyDaemon");
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

    private bool ifIdle;
    public void letDroneFlyByPath(List<RRTNode> path)
    {
        if (ifIdle)
        {
            Debug.Log(path.Count);
            ifIdle = false;
            StartCoroutine("flyDaemon", path);
        }
    }

    public bool getIfIdle()
    {
        return ifIdle;
    }
}
