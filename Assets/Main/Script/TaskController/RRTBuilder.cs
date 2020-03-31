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

        randomPosition = (0, 0);
        minDisNode = root;
    }

    // Update is called once per frame

    private RRTNode minDisNode;
    private (float, float) randomPosition;
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
            for (int i = 1; i <= 8; i++)
            {
                distances[i - 1] = distanceCounter.getDistance(i);
            }
            State currState = distanceCounter.getCurrentStateWhileFindingDistance();
            map.updateMap(distances, currState.getCurrentOrientation(), currState.getCurrentPosition() + new Vector3(0, 0, 1));

            while (uiManager.getGameMode() == InterfaceManager.SELF_DRIVING_MODE)
            {
                float angle;
                Vector3 beforePosition;
                Vector3 afterPosition;
                int deadlockBreaker = 0;
                while (true)
                {
                    this.randomPosition = randomPoint(); // randomly generate a point
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
                    if (randomPosition.Item1 >= minDisNode.X())
                    {
                        angle = radian * 180 / Mathf.PI;
                    }
                    else
                    {
                        angle = radian * 180 / Mathf.PI - 180;
                    }
                    Vector3 unitVector = new Vector3(this.randomPosition.Item1 - this.minDisNode.X(), 0, this.randomPosition.Item2 - this.minDisNode.Z());
                    unitVector = unitVector.normalized;
                    beforePosition = new Vector3(minDisNode.X(), 0, minDisNode.Z());
                    afterPosition = beforePosition + (unitVector * EPS);
                    //demoGraph.drawLine((int)beforePosition.x, (int)beforePosition.z, (int)afterPosition.x, (int)afterPosition.z, Scalar.Olive); //测试
                    if (map.ifThisPointIsChecked(afterPosition))
                    {
                        break;
                    }
                    if(deadlockBreaker >= 8)// detect deadlock and break
                    {
                        rotation.setRotatedAngle(UnityEngine.Random.Range(0.0f, 360.0f));
                        break;
                    }
                    else
                    {
                        deadlockBreaker++;
                    }
                }
                if (!map.ifItIsPossibleThisWayPassable(afterPosition.x, afterPosition.z, beforePosition.x, beforePosition.z))
                {
                    //Debug.Log(map.ifThisPointIsChecked(beforePosition));
                    //Debug.Log(map.ifThisLineIsChecked(beforePosition, afterPosition));
                    if (map.ifThisLineIsChecked(beforePosition, afterPosition))
                    {
                        if (!ifMoreThanFiveNodesInThisArea(afterPosition))
                        {
                            RRTNode newNode = new RRTNode(afterPosition.x, afterPosition.z, minDisNode);
                            theRRT.Add(newNode);
                            demoGraph.addNode(newNode);
                        }
                        break;
                    }
                    else
                    {
                        if (drone.getIfIdle())
                        {
                            RRTNode nearestNode = findNearestNode();
                            letDroneFly(nearestNode, minDisNode);
                            rotation.setRotatedAngle(90 - angle);
                        }
                        //Debug.Log("fly"+ minDisNode.X()+":"+ minDisNode.Z());
                        break;
                    }
                }
            }
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

    private int[,] nodesCount = new int[9, 7];
    private bool ifMoreThanFiveNodesInThisArea(Vector3 position)
    {
        int areaX = Mathf.FloorToInt((position.x - 25) / 50);
        int areaY = Mathf.FloorToInt((position.z - 50) / 50);
        int count = nodesCount[areaX, areaY];
        if(count >= 5)
        {
            return true;
        }
        else
        {
            nodesCount[areaX, areaY] += 1;
            return false;
        }
    }

    public void resetRRT()
    {
        nodesCount = new int[9, 7];
        theRRT = new List<RRTNode>();
        map.resetMap();
    }
}
