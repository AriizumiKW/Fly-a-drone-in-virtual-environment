using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using UnityEngine;

public class RRTBuilder : MonoBehaviour
{
    // this gameobject: the drone
    public const float EPS = 30.0f;
    public DistanceCounter distanceCounter;
    private RotationSimulator rotation;

    private RRTNode root;
    private List<RRTNode> theRRT; // Rapidly-exploring Random Tree
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        rotation = this.gameObject.GetComponent<RotationSimulator>();
        root = new RRTNode(50, 75);
        theRRT.Add(root);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        float originalX = this.transform.position.x;
        float originalZ = this.transform.position.y;
        if (timer >= 0.5f) // run per 0.5 second
        {
            timer = 0;
            (float, float) randomPosition = randomPointWithDistance(originalX, originalZ, EPS); // randomly select a point with distance EPS
            RRTNode minDisNode = root;
            foreach (RRTNode node in theRRT) // find the node in RRT which has the min distance to the random point
            {
                float minDistance = minDisNode.distanceTo(originalX, originalZ);
                float distance = node.distanceTo(originalX, originalZ);
                if(distance < minDistance)
                {
                    minDisNode = node;
                }
            }
            
            float angle = Mathf.Atan(randomPosition.Item2 - originalZ / randomPosition.Item1 - originalX) * 180 / Mathf.PI;
            rotation.setRotatedAngle(90 - angle);
            if(distanceCounter.getDistance() <= EPS)
            {

            }
            else
            {
                
            }
        }
    }

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

    private void letDroneFlyToPosition(float targetX, float targetZ)
    {
        // 测试代码
        float currY = this.transform.position.y;
        this.transform.position = new Vector3(targetX, currY, targetZ);
    }
}
