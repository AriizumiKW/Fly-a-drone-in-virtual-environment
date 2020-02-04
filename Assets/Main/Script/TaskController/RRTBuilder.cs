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
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        rotation = this.gameObject.GetComponent<RotationSimulator>();
        root = new RRTNode(50, 75);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        float originX = this.transform.position.x;
        float originZ = this.transform.position.y;
        if (timer >= 0.5f) // run per 0.5 second
        {
            timer = 0;
            if(distanceCounter.getDistance() < EPS)
            {

            }
            (float, float) randomPosition = randomPointWithDistance(originX, originZ, EPS);

            float angle = Mathf.Atan(randomPosition.Item2 / randomPosition.Item1) * 180 / Mathf.PI;
            rotation.setRotatedAngle(90 - angle);
        }
    }

    private (float,float) randomPointWithDistance(float orginalX, float originalZ, float distance)
    {
        // original point: (25,50). Diagonal point:(475,400).
        int randomX = UnityEngine.Random.Range(30, 470); // range: [30,470)

        byte[] randomBytes = new byte[4];
        RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        rngCrypto.GetBytes(randomBytes);
        int randomZ = BitConverter.ToInt32(randomBytes, 0);
        randomZ = (randomZ % 340) + 55; // range: [55,395)

        float theta = Mathf.Atan(((float)randomZ - originalZ) / ((float)randomX - orginalX));
        float x = distance * Mathf.Cos(theta);
        float z = distance * Mathf.Sin(theta);
        return (x, z);
    }

    private void letDroneFlyToPosition(float targetX, float targetZ)
    {
        // 测试代码
        float currY = this.transform.position.y;
        this.transform.position = new Vector3(targetX, currY, targetZ);
    }
}
