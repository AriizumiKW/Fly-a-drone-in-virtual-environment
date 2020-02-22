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
        
    }
    // Update is called once per frame
    void Update()
    {
        float angleInRadian = Mathf.Atan(-1); //358,142 50,75 1.5599
        float angle = angleInRadian * 180 / Mathf.PI; // caculate drone rotation
        Debug.Log(angle);
    }

    private (float, float) randomPoint()
    {
        int randomX = UnityEngine.Random.Range(30, 470); // range: [30,470)

        byte[] randomBytes = new byte[4];
        RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        rngCrypto.GetBytes(randomBytes);
        int randomZ = BitConverter.ToInt32(randomBytes, 0);
        randomZ = Mathf.Abs(randomZ % 340) + 55; // range: [55,395)

        return (randomX, randomZ);
    }
}
