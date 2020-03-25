using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using UnityEngine;
using OpenCvSharp;

public class RRTBuilder : MonoBehaviour
{
    // this gameobject: the drone
    public const float EPS = 12.0f;
    public InterfaceManager uiManager;
    private DistanceCounter distanceCounter;
    private RotationSimulator rotation;

    private RRTNode root; // the root of the tree
    private RRTDrawer demoGraph;
    private MapBuilder map;
    private PowerfulEngine drone;
    private List<RRTNode> theRRT; // Rapidly-exploring Random Tree
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        distanceCounter = this.gameObject.GetComponent<DistanceCounter>();
        rotation = this.gameObject.GetComponent<RotationSimulator>();
        demoGraph = this.gameObject.GetComponent<RRTDrawer>();
        map = this.gameObject.GetComponent<MapBuilder>();
        drone = this.gameObject.GetComponent<PowerfulEngine>();
        root = new RRTNode(50, 75);
        theRRT = new List<RRTNode>();
        theRRT.Add(root);
        demoGraph.addRootNode(root);

        flag = false;
        randomPosition = (0, 0);
        minDisNode = root;
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
        if (timer >= 2) // run per 2 seconds
        {
            timer = 0;

            float[] distances = new float[8];
            for (int i = 1; i <= 8; i++)
            {
                distances[i - 1] = distanceCounter.getDistance(i);
            }
            map.updateMap(distances, transform.eulerAngles.y, transform.position + new Vector3(0, 0, 1));

            while (true)
            {
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
                float radian = Mathf.Atan((this.randomPosition.Item2 - this.minDisNode.Z()) / (this.randomPosition.Item1 - this.minDisNode.X()));
                float angle = radian * 180 / Mathf.PI;
                Vector3 unitVector = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));
                Vector3 beforePosition = new Vector3(minDisNode.X(), 0, minDisNode.Z());
                Vector3 afterPosition = beforePosition + (unitVector * EPS);
                if (! map.ifMeetObstacleAlongTheDirection(afterPosition.x, afterPosition.z, minDisNode.X(), minDisNode.Z()))
                {
                    if (map.ifTargetPointIsChecked(new Vector3(afterPosition.x, 1, afterPosition.z)))
                    {
                        RRTNode newNode = new RRTNode(afterPosition.x, afterPosition.z, minDisNode);
                        theRRT.Add(newNode);
                        demoGraph.addNode(newNode);
                        //Debug.Log("in region" + newNode.X() + ":" + newNode.Z());
                    }
                    else
                    {
                        RRTNode nearestNode = findNearestNode();
                        letDroneFly(nearestNode, minDisNode);
                        rotation.setRotatedAngle(90 - angle);
                        //Debug.Log("fly"+ minDisNode.X()+":"+ minDisNode.Z());
                        break;
                    }
                }
            }
            

            /*
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
            */
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
    
    private void letDroneFly(RRTNode curr, RRTNode dest)  // curr: current position, dest: destination
    {
        /*
        float targetX = dest.X();
        float targetZ = dest.Z();
        float currY = this.transform.position.y;
        this.transform.position = new Vector3(targetX, currY, targetZ);
        */
        
        List<RRTNode> path = findPathOnRRT(curr, dest);
        path.Insert(0, curr);
        drone.letDroneFlyByPath(path);
        
    }

    private List<RRTNode> findPathOnRRT(RRTNode curr, RRTNode dest) // curr: current position, dest: destination
    {
        if(curr == dest)
        {
            
            return new List<RRTNode>();
        }

        List<RRTNode> ancestors = new List<RRTNode>();
        RRTNode r = dest;
        bool b = true;
        ancestors.Add(r);
        while (b) // dont directly use "true", because visual studio will report an unreasonable error
        {
            if(r == root)
            {
                break;
            }
            r = r.Father();
            if(r == curr)
            {
                ancestors.Reverse();
                return ancestors;
            }
            ancestors.Add(r);
            
        }

        List<RRTNode> path = new List<RRTNode>();
        r = curr;
        while (b)
        {
            r = r.Father();
            path.Add(r);
            List<RRTNode> commonParentToDest = new List<RRTNode>();
            for(int i=0; i<ancestors.Count; i++)
            {
                if(ancestors[i] == r)
                {
                    for(int j=i-1; j>=0; j--)
                    {
                        commonParentToDest.Add(ancestors[j]);
                    }
                    foreach(RRTNode node in commonParentToDest)
                    {
                        path.Add(node);
                    }
                    b = false; // dont directly use "break", because visual studio will report an unreasonable error
                }
                else
                {
                    commonParentToDest.Clear();
                }
            }
        }
        return path;
    }

    private RRTNode findNearestNode()
    {
        float currX = this.transform.position.x;
        float currZ = this.transform.position.z;
        float distance = root.distanceTo(currX, currZ);
        RRTNode nearest = root;
        foreach(RRTNode node in theRRT)
        {
            float d = node.distanceTo(currX, currZ);
            if(d < distance && d < EPS)
            {
                nearest = node;
                distance = nearest.distanceTo(currX, currZ);
            }
        }
        return nearest;
    }
}
