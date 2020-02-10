using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using OpenCvSharp;

public class OpenCVDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*
        Mat red = Cv2.ImRead(Application.persistentDataPath + "/template.png");
        Mat wolf = Cv2.ImRead(Application.persistentDataPath + "/pic.jpg");
        //Debug.Log(Application.persistentDataPath);
        Mat result = new Mat();
        double maxVal;
        double minVal;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        Debug.Log("stop1");
        Cv2.MatchTemplate(wolf, red, result, TemplateMatchModes.SqDiffNormed);
        Debug.Log("stop2");
        Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
        Debug.Log("stop3");
        Debug.Log(minLoc);
        using (new Window("image", leftImg))
        {
            Cv2.WaitKey();
        }
        
        (float, float) randomPosition = randomPointWithDistance(13, 19, 1);
        float x = randomPosition.Item1;
        float z = randomPosition.Item2;
        float dis = Mathf.Pow(x - 13, 2) + Mathf.Pow(z - 19, 2);
        Debug.Log(x + "  " + z + "   " + dis);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private (float, float) randomPointWithDistance(float orginalX, float originalZ, float distance)
    {
        // original point: (25,50). Diagonal point:(475,400).
        int randomX = UnityEngine.Random.Range(30, 470); // range: [30,470)

        byte[] randomBytes = new byte[4];
        RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        rngCrypto.GetBytes(randomBytes);
        int randomZ = BitConverter.ToInt32(randomBytes, 0);
        randomZ = (randomZ % 340) + 55; // range: [55,395)

        float theta = Mathf.Atan(((float)randomZ - originalZ) / ((float)randomX - orginalX));
        float x = distance * Mathf.Cos(theta) + orginalX;
        float z = distance * Mathf.Sin(theta) + originalZ;
        return (x, z);
    }
}
