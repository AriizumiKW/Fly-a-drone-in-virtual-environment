using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using UnityEngine;
using OpenCvSharp;

public class RRTBuilder : MonoBehaviour
{
    // this gameobject: the drone
    public const float EPS = 30.0f;
    public InterfaceManager uiManager;
    private DistanceCounter distanceCounter;
    private RotationSimulator rotation;

    private RRTNode root; // the root of the tree
    private RRTDrawer demoGraph;
    private MapBuilder map;
    private List<RRTNode> theRRT; // Rapidly-exploring Random Tree
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        distanceCounter = this.gameObject.GetComponent<DistanceCounter>();
        rotation = this.gameObject.GetComponent<RotationSimulator>();
        demoGraph = this.gameObject.GetComponent<RRTDrawer>();
        map = this.gameObject.GetComponent<MapBuilder>();
        root = new RRTNode(50, 75);
        theRRT = new List<RRTNode>();
        theRRT.Add(root);
        demoGraph.addRootNode(root);

        flag = false;
        randomPosition = (0, 0);
        minDisNode = new RRTNode(50, 75);
    }

    // Update is called once per frame

    private RRTNode minDisNode;
    private (float, float) randomPosition;
    private bool flag;
    void FixedUpdate()
    {
        if (uiManager.getLock()) // if locked, do nothing
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= 0.5) // run per 0.5 second
        {
            timer = 0;

            float[] distances = new float[8];
            for(int i=1; i<=8; i++)
            {
                distances[i - 1] = distanceCounter.getDistance(i);
                if(distances[i - 1] <= 40.0f)
                {
                    float sightAngle = i * DistanceCounter.FIELD_OF_VIEW / 8 - (DistanceCounter.FIELD_OF_VIEW / 2) - (DistanceCounter.FIELD_OF_VIEW / 16);
                    Vector3 currPosition = this.transform.position + new Vector3(0, 0, 1);
                    Vector3 direction = Quaternion.AngleAxis(sightAngle + 180.0f, new Vector3(0, 1, 0)) * this.transform.forward;
                    Vector3 endPoint = currPosition + direction * distances[i - 1];
                    map.setAnObstacle(endPoint);
                }
            }

            if (this.flag) // ignore at the first run time
            {
                float angleInRadian = (float) Math.Atan((this.randomPosition.Item2 - this.minDisNode.Z()) / (this.randomPosition.Item1 - this.minDisNode.X()));
                float newX = (EPS * Mathf.Cos(angleInRadian)) + this.minDisNode.X();
                float newZ = (EPS * Mathf.Sin(angleInRadian)) + this.minDisNode.Z();
                
                if (distanceCounter.getDistance(1) >= EPS)
                { // success
                    RRTNode newNode = new RRTNode(newX, newZ, this.minDisNode);
                    theRRT.Add(newNode);
                    demoGraph.addNode(newNode);
                }
            }
            else
            {
                this.flag = true;
            }

            this.randomPosition = randomPoint(); // randomly select a point with distance EPS
            this.minDisNode = root;
            float minDistance = this.minDisNode.distanceTo(this.randomPosition.Item1, this.randomPosition.Item2);
            foreach (RRTNode node in theRRT) // find the node in RRT which has the min distance to the random point
            {
                float distance = node.distanceTo(this.randomPosition.Item1, this.randomPosition.Item2);
                if (distance < minDistance)
                {
                    this.minDisNode = node;
                }
            }
            float angle = (float)Math.Atan((this.randomPosition.Item2 - this.minDisNode.Z()) / (this.randomPosition.Item1 - this.minDisNode.X())) * 180 / Mathf.PI;

            rotation.setRotatedAngle(90 - angle);
            letDroneFlyToPosition(this.minDisNode.X(), this.minDisNode.Z());
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90 - angle, transform.eulerAngles.z); // 测试用，功能等于上上行

            //Debug.Log(randomPosition.Item2 - minDisNode.Z() / randomPosition.Item1 - minDisNode.X());
            //Debug.Log(angle);
        }
    }
    
    private (float,float) randomPoint()
    {
        int randomX = UnityEngine.Random.Range(25, 475); // range: [25,475)

        byte[] randomBytes = new byte[4];
        RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        rngCrypto.GetBytes(randomBytes);
        int randomZ = BitConverter.ToInt32(randomBytes, 0);
        randomZ = Mathf.Abs(randomZ % 350) + 50; // range: [50,400)

        return (randomX, randomZ);
    }
    
    private void letDroneFlyToPosition(float targetX, float targetZ)
    {
        // 测试代码
        float currY = this.transform.position.y;
        this.transform.position = new Vector3(targetX, currY, targetZ);
    }

    /*
    private (float,float) randomPointWithDistance(float originalX, float originalZ, float distance)
    {
        // original point: (25,50). Diagonal point:(475,400).
        int randomX = UnityEngine.Random.Range(30, 470); // range: [30,470)

        byte[] randomBytes = new byte[4];
        RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        rngCrypto.GetBytes(randomBytes);
        int randomZ = BitConverter.ToInt32(randomBytes, 0);
        randomZ = (randomZ % 340) + 55; // range: [55,395)

        float theta = Mathf.Atan(((float)randomZ - originalZ) / ((float)randomX - originalX));
        float x = distance * Mathf.Cos(theta) + originalX;
        float z = distance * Mathf.Sin(theta) + originalZ;
        return (x, z);
    }
    */
}
