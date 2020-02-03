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

    private Mat leftImg;
    private Mat rightImg;
    private Mat fundMatrix;
    private Mat template;

    // Start is called before the first frame update
    void Awake()
    {
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
        Debug.Log(minLoc);

        return minLoc;
    }

    private void countDistance()
    {

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
