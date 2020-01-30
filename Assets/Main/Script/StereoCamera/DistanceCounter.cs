using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class DistanceCounter : MonoBehaviour
{
    public const float FOCAL_LENGTH = 20.0f; // millimeters
    public const float BASELINE_LENGTH = 1000.0f; // millimeters
    public InterfaceManager uiManager;
    private Mat leftImg;
    private Mat rightImg;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (uiManager.getLock()) // if locked, do nothing
        {
            return;
        }
        // count the distance
    }

    public void setLeftImage(Mat image)
    {
        leftImg = image;
    }

    public void setRightImage(Mat image)
    {
        rightImg = image;
    }

}
