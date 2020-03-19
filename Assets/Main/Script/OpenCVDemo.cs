using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using OpenCvSharp;

public class OpenCVDemo : MonoBehaviour
{
    private Rigidbody physics;
    // Start is called before the first frame update
    void Start()
    {
        physics = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Vector3 unitDirection = new Vector3(0, 0, 6);
        //physics.MovePosition(physics.position + unitDirection * Time.deltaTime);
    }
}
