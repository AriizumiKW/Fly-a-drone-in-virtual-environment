using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using UnityEngine;

public class RRTBuilder : MonoBehaviour
{
    // this gameobject: the drone
    public const float EPS = 30.0f;
    public InterfaceManager uiManager;
    private DistanceCounter distanceCounter;
    private RotationSimulator rotation;

    private RRTNode root; // the root of the tree
    private List<RRTNode> theRRT; // Rapidly-exploring Random Tree
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        distanceCounter = this.gameObject.GetComponent<DistanceCounter>();
        rotation = this.gameObject.GetComponent<RotationSimulator>();
        root = new RRTNode(50, 75);
        theRRT = new List<RRTNode>();
        theRRT.Add(root);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (uiManager.getLock()) // if locked, do nothing
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= 0.5f) // run per 0.5 second
        {
            timer = 0;
            (float, float) randomPosition = randomPoint(); // randomly select a point with distance EPS
            RRTNode minDisNode = root;
            float minDistance = minDisNode.distanceTo(randomPosition.Item1, randomPosition.Item2);
            foreach (RRTNode node in theRRT) // find the node in RRT which has the min distance to the random point
            {
                float distance = node.distanceTo(randomPosition.Item1, randomPosition.Item2);
                if (distance < minDistance)
                {
                    minDisNode = node;
                }
            }
            float angleInRadian = Mathf.Atan(randomPosition.Item2 - minDisNode.Z() / randomPosition.Item1 - minDisNode.X());
            float angle = angleInRadian * 180 / Mathf.PI; // caculate drone rotation
            float newX = EPS * Mathf.Cos(angleInRadian) + minDisNode.X();
            float newZ = EPS * Mathf.Sin(angleInRadian) + minDisNode.Z();

            rotation.setRotatedAngle(90 - angle);
            letDroneFlyToPosition(minDisNode.X(), minDisNode.Z());
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90 - angle, transform.eulerAngles.z);
            if(distanceCounter.getDistance() <= EPS)
            {
                RRTNode newNode = new RRTNode(randomPosition.Item1, randomPosition.Item2, minDisNode);
                theRRT.Add(newNode);
            }
            
        }
    }
    
    private (float,float) randomPoint()
    {
        int randomX = UnityEngine.Random.Range(30, 470); // range: [30,470)

        byte[] randomBytes = new byte[4];
        RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        rngCrypto.GetBytes(randomBytes);
        int randomZ = BitConverter.ToInt32(randomBytes, 0);
        randomZ = (randomZ % 340) + 55; // range: [55,395)

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
