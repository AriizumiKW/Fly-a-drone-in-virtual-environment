using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using UnityEngine;
using OpenCvSharp;

public class RRTBuilder : MonoBehaviour
{
    // this gameobject: the drone
    public const float EPS = 22.0f;
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

        minDisNode = root;
        requestList = new LinkedList<(RRTNode, Vector3)>();
        b = true;
    }

    // Update is called once per frame

    private float flyAngle;
    private RRTNode minDisNode;

    private bool b;
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

            if (uiManager.getGameMode() == InterfaceManager.SELF_DRIVING_MODE)
            {
                if (b)
                {
                    b = false;
                    StartCoroutine("buildingRRT");
                }
                if (drone.getIfIdle())
                {
                    if (requestList.Count != 0)
                    {
                        RRTNode nearestNode = findNearestNode();
                        RRTNode nearestNodeInRequestList = requestList.First.Value.Item1;
                        float minDistance = nearestNodeInRequestList.distanceTo(this.transform.position.x, this.transform.position.z);
                        //foreach ((RRTNode before, Vector3 after) in requestList)
                        var node = requestList.First;
                        while (node != null)
                        {
                            (RRTNode before, Vector3 after) = node.Value;
                            var next = node.Next;
                            float distance = before.distanceTo(this.transform.position.x, this.transform.position.z);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                nearestNodeInRequestList = before;
                            }
                            node = next;
                        }
                        requestList.Clear();
                        letDroneFly(nearestNode, nearestNodeInRequestList);
                        //rotation.setRotatedAngle(90 - flyAngle);
                    }
                }
            }
        }
    }

    private LinkedList<(RRTNode, Vector3)> requestList; // Item1: node in RRT. Item2: the node we want to add into RRT, but perhaps this way is not passable. Request to check

    private IEnumerator buildingRRT()
    {
        while (uiManager.getLock() == false && uiManager.getGameMode() == InterfaceManager.SELF_DRIVING_MODE)
        {
            yield return new WaitForSeconds(0.02f); // try to add a branch each 0.02 seconds
            Vector3 afterPosition = generateRandomPointWithDistanceEPStoMinDisNode();
            Vector3 beforePosition = new Vector3(this.minDisNode.X(), 0, this.minDisNode.Z());

            RRTNode minDisNodeToAfterPosition = root;
            float dis = root.distanceTo(afterPosition.x, afterPosition.z);
            foreach(RRTNode node in theRRT)
            {
                float dis2 = node.distanceTo(afterPosition.x, afterPosition.z);
                if(dis2 < dis && dis2 > EPS/3)
                {
                    dis = dis2;
                    minDisNodeToAfterPosition = node;
                }
            }
            Vector3 minDisNodeToAfterPositionExpressedByVector3 = new Vector3(minDisNodeToAfterPosition.X(), 0, minDisNodeToAfterPosition.Z());
            if (!map.ifMeetAnObstacleAlongThisWay(minDisNodeToAfterPositionExpressedByVector3.x, minDisNodeToAfterPositionExpressedByVector3.z, 
                afterPosition.x, afterPosition.z)) // feasible condition
            {
                if (map.ifThisLineIsChecked(minDisNodeToAfterPositionExpressedByVector3, afterPosition)) // we can go this way, just add it into RRT
                {
                    float randomNum = UnityEngine.Random.Range(0, 1.0f);
                    int howManyPointInThisArea = findHowManyNodesInThisArea(afterPosition);
                    float threshold = thresholdFunction(howManyPointInThisArea);
                    if(randomNum <= threshold) // neighborhood checking
                    {
                        RRTNode newNode = new RRTNode(afterPosition.x, afterPosition.z, minDisNodeToAfterPosition);
                        theRRT.Add(newNode);
                        nodesCountPlusOne(afterPosition);
                        demoGraph.addNode(newNode);
                    }
                }
                else // can not go, add it into requestlist
                {
                    requestList.AddLast((minDisNode, afterPosition));
                }
            }
            else if (!map.ifMeetAnObstacleAlongThisWay(beforePosition.x, beforePosition.z, afterPosition.x, afterPosition.z)) // feasible condition
            {
                if (map.ifThisLineIsChecked(beforePosition, afterPosition)) // we can go this way, just add it into RRT
                {
                    float randomNum = UnityEngine.Random.Range(0, 1.0f);
                    int howManyPointInThisArea = findHowManyNodesInThisArea(afterPosition);
                    float threshold = thresholdFunction(howManyPointInThisArea);
                    if (randomNum <= threshold) // neighborhood checking
                    {
                        RRTNode newNode = new RRTNode(afterPosition.x, afterPosition.z, minDisNodeToAfterPosition);
                        theRRT.Add(newNode);
                        nodesCountPlusOne(afterPosition);
                        demoGraph.addNode(newNode);
                    }
                }
                else // can not go, add it into requestlist
                {
                    requestList.AddLast((minDisNode, afterPosition));
                }
            }
        }
    }

    //private (float, float) rp; // uesd to test

    private Vector3 generateRandomPointWithDistanceEPStoMinDisNode()
    {
        Vector3 beforePosition;
        Vector3 afterPosition;
        (float, float) randomPosition;
        int deadlockBreaker = 0;
        while (true)
        {
            randomPosition = randomPoint(); // randomly generate a point
            this.minDisNode = root;
            float minDistance = this.minDisNode.distanceTo(randomPosition.Item1, randomPosition.Item2);
            foreach (RRTNode node in theRRT) // find the node in RRT which has the min distance to the random point
            {
                float distance = node.distanceTo(randomPosition.Item1, randomPosition.Item2);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    this.minDisNode = node;
                }
            }

            float radian = Mathf.Atan((randomPosition.Item2 - this.minDisNode.Z()) / (randomPosition.Item1 - this.minDisNode.X()));
            if (randomPosition.Item1 >= minDisNode.X())
            {
                this.flyAngle = radian * 180 / Mathf.PI;
            }
            else
            {
                this.flyAngle = radian * 180 / Mathf.PI - 180;
            }
            Vector3 unitVector = new Vector3(randomPosition.Item1 - this.minDisNode.X(), 0, randomPosition.Item2 - this.minDisNode.Z());
            unitVector = unitVector.normalized;
            beforePosition = new Vector3(minDisNode.X(), 0, minDisNode.Z());
            afterPosition = beforePosition + (unitVector * EPS);
            //demoGraph.drawLine((int)beforePosition.x, (int)beforePosition.z, (int)afterPosition.x, (int)afterPosition.z, Scalar.Olive); //测试
            if (map.ifThisPointIsChecked(afterPosition))
            {
                break;
            }
            if (deadlockBreaker >= 8)// detect deadlock and break
            {
                rotation.setRotatedAngle(UnityEngine.Random.Range(0.0f, 360.0f));
                break;
            }
            else
            {
                deadlockBreaker++;
            }
        }
        //rp = randomPosition;
        return afterPosition;
    }

    private float thresholdFunction(int x) // if the number of nodes near new node is bigger than 15, the probability will only be 0.04, otherwise p = -0.06x + 1
    {
        if(x >= 15)
        {
            return 0.04f;
        }
        else
        {
            float y = -0.06f * (float)x + 1; // choose linear function (Occam's Razor)
            return y;
        }
    }
    
    private (float,float) randomPoint() // real random num
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
    private int findHowManyNodesInThisArea(Vector3 position)
    {
        int areaX = Mathf.FloorToInt((position.x - 25) / 50);
        int areaY = Mathf.FloorToInt((position.z - 50) / 50);
        if(areaX < 0)
        {
            areaX = 0;
        }
        else if(areaX > 8)
        {
            areaX = 8;
        }
        if(areaY < 0)
        {
            areaY = 0;
        }
        else if(areaY > 6)
        {
            areaY = 6;
        }
        int count = nodesCount[areaX, areaY];
        return count;
    }

    private void nodesCountPlusOne(Vector3 position)
    {
        int areaX = Mathf.FloorToInt((position.x - 25) / 50);
        int areaY = Mathf.FloorToInt((position.z - 50) / 50);
        if (areaX < 0)
        {
            areaX = 0;
        }
        else if (areaX > 8)
        {
            areaX = 8;
        }
        if (areaY < 0)
        {
            areaY = 0;
        }
        else if (areaY > 6)
        {
            areaY = 6;
        }
        nodesCount[areaX, areaY] += 1;
    }

    public void resetRRT()
    {
        nodesCount = new int[9, 7];
        theRRT = new List<RRTNode>();
        map.resetMap();
    }
}
