using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using OpenCvSharp;

public class OpenCVDemo : MonoBehaviour
{
    //Mat fundMatrix;
    // Start is called before the first frame update
    void Start()
    {
        float FIELD_OF_VIEW = 61.9f;
        for(int whichPart=1; whichPart <= 8; whichPart++)
        {
            float angle = whichPart * FIELD_OF_VIEW / 8 - (FIELD_OF_VIEW / 2) - (FIELD_OF_VIEW / 16) + 90;
            Debug.Log(angle+":"+Mathf.Sin(angle*Mathf.PI/180));
        }
        /*
        bool leftAlready = true;
        bool rightAlready = true;
        if (leftAlready && rightAlready) // 新版距离计算
        {
            Point2f[] leftMatchPoints =
            {
                new Point2f(54,31), //1,1
                new Point2f(116,98), //2,2
                new Point2f(180,162), //3,3
                new Point2f(242,229), //4,4
                new Point2f(304,294), //5,5
                new Point2f(367,361), //6,6
                new Point2f(304,31), //5,1
                new Point2f(367,97), //6,2
                new Point2f(428,162), //7,3
            };

            Point2f[] rightMatchPoints =
            {
                new Point2f(30,31), //1,1
                new Point2f(92,98), //2,2
                new Point2f(156,162), //3,3
                new Point2f(217,229), //4,4
                new Point2f(281,294), //5,5
                new Point2f(343,361), //6,6
                new Point2f(280,31), //5,1
                new Point2f(343,97), //6,2
                new Point2f(404,162), //7,3
            };

            InputArray left = InputArray.Create(leftMatchPoints);
            InputArray right = InputArray.Create(rightMatchPoints);

            fundMatrix = Cv2.FindFundamentalMat(left, right, FundamentalMatMethod.Point8);
            
            float[] l_array = {47, 204, 1};
            Point3d left_point = new Point3d(48, 204, 1);

            Mat leftP = new Mat();
            leftP.Add(left_point);
            Mat epliline = new Mat();
            
            //float a = 368 * fundMatrix.At<float>(0, 0) + 204 * fundMatrix.At<float>(1, 0) + fundMatrix.At<float>(2, 0);
            //float b = 368 * fundMatrix.At<float>(0, 1) + 204 * fundMatrix.At<float>(1, 1) + fundMatrix.At<float>(2, 1);
            //float c = 368 * fundMatrix.At<float>(0, 2) + 204 * fundMatrix.At<float>(1, 2) + fundMatrix.At<float>(2, 2);
            //Debug.Log(a + ":" + b + ":" + c);

            //float y1 = (-a * 0 - c) / b;
            //float y2 = (-a * 100 - c) / b;
            //Debug.Log(y1 + ":" + y2);
            
            Cv2.ComputeCorrespondEpilines(leftP, 1, fundMatrix, epliline);
            //Debug.Log(epliline);
            //Debug.Log(epliline.At<Point3d>(0).ToString());
            double x = epliline.At<Point3d>(0).X;
            double y = epliline.At<Point3d>(0).Y;
            double z = epliline.At<Point3d>(0).Z;


            Mat rightImg = Cv2.ImRead(Application.persistentDataPath + "/R.png");
            Cv2.Line(rightImg, new Point(0, -z / y), new Point(1000, (-x * 1000 - z) / y), Scalar.Green);
            Cv2.ImWrite(Application.persistentDataPath + "/1233333.png", rightImg);
            //Debug.Log(fundMatrix.ToString());
            */
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
