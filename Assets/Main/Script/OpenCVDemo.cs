using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using OpenCvSharp;
using System.Threading.Tasks;

public class OpenCVDemo : MonoBehaviour
{

    void Start()
    {
        /*
        Mat fmat = initFundMatrix();
        Mat epiline = new Mat();
        Mat center = new Mat();
        center.Add(new Point3d(590, 295, 1));
        Cv2.ComputeCorrespondEpilines(center, 1, fmat, epiline);
        Mat plate = Cv2.ImRead(Application.persistentDataPath + "/test output.png");
        drawEpipolarLine(plate, epiline, center, Scalar.LightBlue);
        Cv2.ImWrite(Application.persistentDataPath + "/test output.png", plate);
        */

        /*
        Mat input = Cv2.ImRead(Application.persistentDataPath + "/prepro.png");
        affineTest(input);
        */

        //testHeight(1);
    }

    private Mat initFundMatrix() // initialize fundamental matrix
    {
        Point3f[] leftMatchPoints =
            {
                new Point3f(54,31,1), //1,1
                new Point3f(116,98,1), //2,2
                new Point3f(180,162,1), //3,3
                new Point3f(242,229,1), //4,4
                new Point3f(304,294,1), //5,5
                new Point3f(367,361,1), //6,6
                new Point3f(304,31,1), //5,1
                new Point3f(367,97,1), //6,2
                new Point3f(428,162,1), //7,3
            };
        Point3f[] rightMatchPoints =
            {
                new Point3f(30,31,1), //1,1
                new Point3f(92,98,1), //2,2
                new Point3f(156,162,1), //3,3
                new Point3f(217,229,1), //4,4
                new Point3f(281,294,1), //5,5
                new Point3f(343,361,1), //6,6
                new Point3f(280,31,1), //5,1
                new Point3f(343,97,1), //6,2
                new Point3f(404,162,1), //7,3
            };
        InputArray leftInput = InputArray.Create(leftMatchPoints);
        InputArray rightInput = InputArray.Create(rightMatchPoints);
        return Cv2.FindFundamentalMat(leftInput, rightInput, FundamentalMatMethod.Point8);
    }

    private void drawEpipolarLine(Mat plate, Mat epliline, Mat point, Scalar color)// use for testing only
    {
        double x = epliline.At<Point3d>(0).X;
        double y = epliline.At<Point3d>(0).Y;
        double z = epliline.At<Point3d>(0).Z;
        double px = point.At<Point3d>(0, 0).X;
        double py = point.At<Point3d>(0, 0).Y;
        Cv2.Line(plate, new Point(0, -z / y), new Point(800 - 1, (-x * (800 - 1) - z) / y), color);
        Cv2.Circle(plate, new Point(px, py), 5, color, -1, LineTypes.Link8);
    }

    private void affineTest(Mat input)
    {
        for (int col = 0; col < input.Cols; col++) // affine transformation
        {
            int up = Mathf.RoundToInt(202.0f * col / 797.0f);
            for (int row = input.Rows - 1; row >= 230; row--)
            {
                try
                {
                    input.Set<Vec3b>(row, col, input.At<Vec3b>(row - up, col));
                }
                catch (Exception e)
                {
                    Debug.Log(row + ":" + col);
                }
            }
        }
        Cv2.ImWrite(Application.persistentDataPath + "/affine.png", input);
    }

    private void testHeight(int halfHeight) // test, find which height is the most appropriate
    {
        Mat right = Cv2.ImRead(Application.persistentDataPath + "/test-r.png");
        int upper = 225 - halfHeight;
        int buttom = 225 + halfHeight;
        Mat right_processed = new Mat(halfHeight * 2, 800, MatType.CV_8UC3);
        for(int i=0; i<right_processed.Rows; i++)
        {
            for(int j=0; j<right_processed.Cols; j++)
            {
                right_processed.Set<Vec3b>(i, j, right.Get<Vec3b>(i + upper, j));
            }
        }

        Mat left = Cv2.ImRead(Application.persistentDataPath + "/test-l.png");
        Mat left_processed = new Mat(halfHeight * 2, 800, MatType.CV_8UC3);
        for (int i = 0; i < left_processed.Rows; i++)
        {
            for (int j = 0; j < left_processed.Cols; j++)
            {
                left_processed.Set<Vec3b>(i, j, left.Get<Vec3b>(i + upper, j));
            }
        }
        Mat part1 = left_processed.ColRange(0, 99);
        Mat part2 = left_processed.ColRange(100, 199);
        Mat part3 = left_processed.ColRange(200, 299);
        Mat part4 = left_processed.ColRange(300, 399);
        Mat part5 = left_processed.ColRange(400, 499);
        Mat part6 = left_processed.ColRange(500, 599);
        Mat part7 = left_processed.ColRange(600, 699);
        Mat part8 = left_processed.ColRange(700, 799);
        Mat[] mats = { part1, part2, part3, part4, part5, part6, part7, part8};

        Mat result = new Mat();
        double maxVal;
        double minVal;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        Point[] points = new Point[8];
        for(int i=0; i<8; i++)
        {
            Cv2.MatchTemplate(right_processed, mats[i], result, TemplateMatchModes.SqDiffNormed);
            Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
            points[i] = new Point(minLoc.X + mats[i].Cols / 2, minLoc.Y + mats[i].Rows / 2);
        }

        int[] disparities = new int[8];
        for (int i = 0; i < 8; i++)
        {
            int disparity = points[i].X - (50 + i * 100);
            disparities[i] = disparity;
        }

        Debug.Log(String.Join(", ", disparities));
    }
}
