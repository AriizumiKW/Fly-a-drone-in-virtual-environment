using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class DistanceCounter : MonoBehaviour
{
    public const float FOCAL_LENGTH = 20.0f; // millimeters
    public const float BASELINE_LENGTH = 1000.0f; // millimeters
    public InterfaceManager uiManager;
    //public Texture2D templateTexture2DFormat;

    private float timer;
    private Mat leftImg;
    private Mat rightImg;
    private Mat fundMatrix;
    private Mat template;

    // Start is called before the first frame update
    void Awake()
    {
        timer = 0;
        initFundamentalMatrix();
        //template = Mat.FromImageData(templateTexture2DFormat.EncodeToPNG()); // convert to OpenCv.Mat data type
        template = Cv2.ImRead(Application.persistentDataPath + "/template.png");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (uiManager.getLock()) // if locked, do nothing
        {
            return;
        }
        // count the distance
        //Point matchPoint = matchingLeftImage();

        timer += Time.deltaTime;
        if(timer >= 1.0f)
        {
            Point matchPoint = matchingLeftImage(); // run once per second
            Mat epiline = countEpiline(matchPoint);
            drawEpiline(rightImg, epiline);
            timer = 0;
        }
    }

    private void initFundamentalMatrix()
    {
        // these matching points I conducted manually.
        Mat leftImgMatchingPoints = new Mat();
        Mat rightImgMatchingPoints = new Mat();

        leftImgMatchingPoints.Add(new Point2f(278, 195));
        leftImgMatchingPoints.Add(new Point2f(278, 215));
        leftImgMatchingPoints.Add(new Point2f(314, 200));
        leftImgMatchingPoints.Add(new Point2f(315, 221));
        leftImgMatchingPoints.Add(new Point2f(352, 189));
        leftImgMatchingPoints.Add(new Point2f(352, 210));
        leftImgMatchingPoints.Add(new Point2f(386, 192));
        leftImgMatchingPoints.Add(new Point2f(387, 212));

        rightImgMatchingPoints.Add(new Point2f(239, 195));
        rightImgMatchingPoints.Add(new Point2f(239, 216));
        rightImgMatchingPoints.Add(new Point2f(275, 200));
        rightImgMatchingPoints.Add(new Point2f(275, 220));
        rightImgMatchingPoints.Add(new Point2f(312, 190));
        rightImgMatchingPoints.Add(new Point2f(313, 210));
        rightImgMatchingPoints.Add(new Point2f(347, 192));
        rightImgMatchingPoints.Add(new Point2f(348, 213));

        Debug.Log(leftImgMatchingPoints);
        fundMatrix = Cv2.FindFundamentalMat(leftImgMatchingPoints, rightImgMatchingPoints, FundamentalMatMethod.Point8);
    }

    private Point matchingLeftImage()
    {
        // image matching
        Mat result = new Mat();
        double maxVal;
        double minVal;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        Cv2.MatchTemplate(leftImg, template, result, TemplateMatchModes.SqDiffNormed);
        Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
        //Debug.Log(minLoc);
        /*
        Cv2.Rectangle(leftImg, minLoc, new Point(minLoc.X + template.Cols, minLoc.Y + template.Rows), new Scalar(0, 255, 0), 2, LineTypes.Link8, 0);
        Cv2.ImWrite(Application.persistentDataPath + "/match.png", leftImg);
        */
        return new Point(minLoc.X + template.Cols / 2, minLoc.Y + template.Rows / 2); // center point of matcing rectangle
    }

    private Mat countEpiline(Point p)
    {
        Mat epliline = new Mat();
        Mat leftPoint = new Mat();
        leftPoint.Add(p);
        Cv2.ComputeCorrespondEpilines(leftPoint, 1, this.fundMatrix, epliline);
        //Debug.Log(epliline);
        return epliline;
    }

    private void countParallax()
    {

    }

    private void drawEpiline(Mat drawImg, Mat epiline)
    {
        // used to demonstrate only
        Mat result = new Mat();
        Vec3f epl = epiline.Get<Vec3f>(0, 0);
        // expression: ax + by + c = 0
        float a = epl.Item0;
        float b = epl.Item1;
        float c = epl.Item2;
        float endPointX = ImageStreamCapturer.SCREEN_WIDTH;
        Debug.Log(epl);
        Point startPoint = new Point(0, -c / b); // when x=0, y=-c/b.
        Point endPoint = new Point(endPointX, (-a * endPointX - c) / b); // y=(-ax-c)/b

        int thickness = 2;
        Cv2.Line(drawImg, startPoint, endPoint, new Scalar(0, 255, 0), thickness, LineTypes.Link8);
        Cv2.ImWrite(Application.persistentDataPath + "/epiline.png", drawImg);
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
