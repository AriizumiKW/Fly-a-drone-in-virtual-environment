using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class DistanceCounter : MonoBehaviour
{
    public const float FOCAL_LENGTH = 20.0f; // millimeters
    public const float FIELD_OF_VIEW = 61.9f; // field of view, in angle
    public const float BASELINE_LENGTH = 1000.0f; // millimeters
    public const int SCREEN_WIDTH = 800;
    public const int SCREEN_HEIGHT = 450;
    public float COEFF = 0.0233f; // coefficient
    public int test = 0;
    public InterfaceManager uiManager;
    //public Texture2D templateTexture2DFormat;

    private float[] distances;
    private Mat leftImg; // 800 * 450 pixels
    private Mat rightImg; // 800 * 450 pixels
    private Mat fundMatrix;

    // Start is called before the first frame update
    void Start()
    {
        distances = new float[8];
        initFundMatrix();
    }

    // Update is called once per frame
    public void countDistance()
    {
        if (uiManager.getLock()) // if locked, do nothing
        {
            return;
        }

        //drawLineInUnity(distance);
        if (leftAlready && rightAlready)
        {
            leftAlready = false;
            rightAlready = false;
            cutLeftImageToEightParts();
            findEplilines();
            double[] disparities = findDisparity();
            for(int i = 0; i <= 7; i++)
            {
                distances[i] = calculateDistance(disparities[i], i + 1);
            }
            drawLineInUnity(distances[test], test+1);
            //Debug.Log(string.Join("", disparities));
            //Debug.Log(string.Join("",distances));
        }
    }
    private void initFundMatrix() // initialize fundamental matrix
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
        InputArray leftInput = InputArray.Create(leftMatchPoints);
        InputArray rightInput = InputArray.Create(rightMatchPoints);
        this.fundMatrix = Cv2.FindFundamentalMat(leftInput, rightInput, FundamentalMatMethod.Point8);
    }

    Mat part1;
    Mat part2;
    Mat part3;
    Mat part4;
    Mat part5;
    Mat part6;
    Mat part7;
    Mat part8;
    Mat part1_centre;
    Mat part2_centre;
    Mat part3_centre;
    Mat part4_centre;
    Mat part5_centre;
    Mat part6_centre;
    Mat part7_centre;
    Mat part8_centre;
    Mat part1_epliline;
    Mat part2_epliline;
    Mat part3_epliline;
    Mat part4_epliline;
    Mat part5_epliline;
    Mat part6_epliline;
    Mat part7_epliline;
    Mat part8_epliline;
    private void cutLeftImageToEightParts() // step1, choose the centre region of left image, cut it into 8 equal parts
    {
        int topHeight = (int)(0.475 * leftImg.Rows);
        int buttomHeight = (int)(0.525 * leftImg.Rows);
        Mat leftImg_roi = leftImg.RowRange(topHeight, buttomHeight); // our region of interest, which is the centre region of left image
        part1 = leftImg_roi.ColRange((int)(0.1 * leftImg.Cols), (int)(0.2 * leftImg.Cols));
        part2 = leftImg_roi.ColRange((int)(0.2 * leftImg.Cols), (int)(0.3 * leftImg.Cols));
        part3 = leftImg_roi.ColRange((int)(0.3 * leftImg.Cols), (int)(0.4 * leftImg.Cols));
        part4 = leftImg_roi.ColRange((int)(0.4 * leftImg.Cols), (int)(0.5 * leftImg.Cols));
        part5 = leftImg_roi.ColRange((int)(0.5 * leftImg.Cols), (int)(0.6 * leftImg.Cols));
        part6 = leftImg_roi.ColRange((int)(0.6 * leftImg.Cols), (int)(0.7 * leftImg.Cols));
        part7 = leftImg_roi.ColRange((int)(0.7 * leftImg.Cols), (int)(0.8 * leftImg.Cols));
        part8 = leftImg_roi.ColRange((int)(0.8 * leftImg.Cols), (int)(0.9 * leftImg.Cols));
        part1_centre = new Mat();
        part2_centre = new Mat();
        part3_centre = new Mat();
        part4_centre = new Mat();
        part5_centre = new Mat();
        part6_centre = new Mat();
        part7_centre = new Mat();
        part8_centre = new Mat();
        part1_centre.Add(new Point3d((int)(0.15 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part2_centre.Add(new Point3d((int)(0.25 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part3_centre.Add(new Point3d((int)(0.35 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part4_centre.Add(new Point3d((int)(0.45 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part5_centre.Add(new Point3d((int)(0.55 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part6_centre.Add(new Point3d((int)(0.65 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part7_centre.Add(new Point3d((int)(0.75 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part8_centre.Add(new Point3d((int)(0.85 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
    }

    private void findEplilines() // step2, find eplilines for 8 centre points
    {
        part1_epliline = new Mat();
        part2_epliline = new Mat();
        part3_epliline = new Mat();
        part4_epliline = new Mat();
        part5_epliline = new Mat();
        part6_epliline = new Mat();
        part7_epliline = new Mat();
        part8_epliline = new Mat();
        Cv2.ComputeCorrespondEpilines(part1_centre, 1, this.fundMatrix, part1_epliline);
        Cv2.ComputeCorrespondEpilines(part2_centre, 1, this.fundMatrix, part2_epliline);
        Cv2.ComputeCorrespondEpilines(part3_centre, 1, this.fundMatrix, part3_epliline);
        Cv2.ComputeCorrespondEpilines(part4_centre, 1, this.fundMatrix, part4_epliline);
        Cv2.ComputeCorrespondEpilines(part5_centre, 1, this.fundMatrix, part5_epliline);
        Cv2.ComputeCorrespondEpilines(part6_centre, 1, this.fundMatrix, part6_epliline);
        Cv2.ComputeCorrespondEpilines(part7_centre, 1, this.fundMatrix, part7_epliline);
        Cv2.ComputeCorrespondEpilines(part8_centre, 1, this.fundMatrix, part8_epliline);
    }

    private double[] findDisparity() // step3, find each disparity for 8 centre point
    {
        double disparity1 = findMatchPoint(part1, part1_epliline).Item1 - part1_centre.At<Point3d>(0).X;
        double disparity2 = findMatchPoint(part2, part2_epliline).Item1 - part2_centre.At<Point3d>(0).X;
        double disparity3 = findMatchPoint(part3, part3_epliline).Item1 - part3_centre.At<Point3d>(0).X;
        double disparity4 = findMatchPoint(part4, part4_epliline).Item1 - part4_centre.At<Point3d>(0).X;
        double disparity5 = findMatchPoint(part5, part5_epliline).Item1 - part5_centre.At<Point3d>(0).X;
        double disparity6 = findMatchPoint(part6, part6_epliline).Item1 - part6_centre.At<Point3d>(0).X;
        double disparity7 = findMatchPoint(part7, part7_epliline).Item1 - part7_centre.At<Point3d>(0).X;
        double disparity8 = findMatchPoint(part8, part8_epliline).Item1 - part8_centre.At<Point3d>(0).X;
        double[] array = { disparity1, disparity2, disparity3, disparity4, disparity5, disparity6, disparity7, disparity8 };
        return array;
    }

    private (double, double) findMatchPoint(Mat templateFromLeftImg, Mat epliline)
    {
        Mat rightImg_roi = new Mat(templateFromLeftImg.Rows, SCREEN_WIDTH, MatType.CV_8UC3);
        double a = epliline.At<Point3d>(0).X;
        double b = epliline.At<Point3d>(0).Y;
        double c = epliline.At<Point3d>(0).Z; // ax+by+cz=0 (homogeneous coordinates)
        for (int col = 0; col < SCREEN_WIDTH; col++) // affine transformation
        {
            double y = (-c - a * col)/ b;
            for (int row = (int)(y - rightImg_roi.Rows/2); row < (int)(y + rightImg_roi.Rows/2); row++)
            {
                rightImg_roi.Set<Vec3b>(row - (int)(y - rightImg_roi.Rows / 2), col, rightImg.Get<Vec3b>(row, col));
            }
        }
        Mat result = new Mat();
        double maxVal;
        double minVal;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        Cv2.MatchTemplate(rightImg_roi, templateFromLeftImg, result, TemplateMatchModes.SqDiffNormed);
        Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
        /*
        if(templateFromLeftImg == part1)
        {
            Cv2.Circle(rightImg_roi, minLoc.X + templateFromLeftImg.Cols / 2, minLoc.Y + templateFromLeftImg.Rows / 2, 2, Scalar.Red, -1, LineTypes.Link8);
            Cv2.ImWrite(Application.persistentDataPath + "/test output.png", rightImg_roi);
        }
        */
        return (minLoc.X + templateFromLeftImg.Cols / 2, minLoc.Y + templateFromLeftImg.Rows / 2); // center point of matcing rectangle
    }

    private float calculateDistance(double disparity, int whichPart)
    {
        float angle = whichPart * FIELD_OF_VIEW / 8 - (FIELD_OF_VIEW / 2) - (FIELD_OF_VIEW / 16) + 90;
        float distance = COEFF * FOCAL_LENGTH * BASELINE_LENGTH / (float)disparity;
        return Mathf.Abs(distance / Mathf.Sin(angle * Mathf.PI / 180));
    }

    private void drawLineInUnity(float distance, int whichPart)// use for testing only
    {
        //Debug.Log(distance);
        float angle = whichPart * FIELD_OF_VIEW / 8 - (FIELD_OF_VIEW / 2) - (FIELD_OF_VIEW / 16);
        Vector3 laserPosition = this.transform.position + new Vector3(0, 0, 1);
        Vector3 direction = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0)) * this.transform.forward;
        Vector3 endPoint = laserPosition + direction * distance;
        Debug.DrawLine(laserPosition, endPoint);
    }

    private void outputImageFile(Mat image, Point center, int type)// use for testing only
    {
        // type=1, left image. type=2, right image
        Mat image2 = image;
        Cv2.Circle(image2, center, 2, new Scalar(0, 255, 0));

        string filename;
        if (type == 1)
        {
            filename = Application.persistentDataPath + "/a left img-match.png";
        }
        else if (type == 2)
        {
            filename = Application.persistentDataPath + "/a right img-match.png";
        }
        else
        {
            filename = Application.persistentDataPath + "/no tag.png-match";
        }
        Cv2.ImWrite(filename, image2);
    }

    private void drawEpipolarLine(Mat plate, Mat epliline)// use for testing only
    {
        double x = epliline.At<Point3d>(0).X;
        double y = epliline.At<Point3d>(0).Y;
        double z = epliline.At<Point3d>(0).Z;
        Cv2.Line(plate, new Point(0, -z / y), new Point(SCREEN_WIDTH - 1, (-x * (SCREEN_WIDTH - 1) - z) / y), Scalar.Green);
        Cv2.ImWrite(Application.persistentDataPath + "/test output.png", plate);
    }

    private bool leftAlready = false;
    private bool rightAlready = false;
    public void setLeftImage(Mat image)
    {
        leftImg = image;
        leftAlready = true;
    }

    public void setRightImage(Mat image)
    {
        rightImg = image;
        rightAlready = true;
    }

    public float getDistance(int whichPart)
    {
        return distances[whichPart - 1];
    }
}
