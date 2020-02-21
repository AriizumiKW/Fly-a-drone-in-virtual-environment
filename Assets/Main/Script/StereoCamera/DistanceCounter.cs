using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class DistanceCounter : MonoBehaviour
{
    public const float FOCAL_LENGTH = 20.0f; // millimeters
    public const float BASELINE_LENGTH = 1000.0f; // millimeters
    public float COEFF = 0.0208f; // coefficient
    public InterfaceManager uiManager;
    //public Texture2D templateTexture2DFormat;

    private float timer;
    private float distance;
    private Mat leftImg;
    private Mat rightImg;
    private Mat fundMatrix;
    private Mat template;

    // Start is called before the first frame update
    void Awake()
    {
        timer = 0;
        //template = Mat.FromImageData(templateTexture2DFormat.EncodeToPNG()); // convert to OpenCv.Mat data type
        template = Cv2.ImRead(Application.persistentDataPath + "/template.png");
    }

    // Update is called once per frame
    public void countDistance()
    {
        if (uiManager.getLock()) // if locked, do nothing
        {
            return;
        }
        // count the distance
        //Point matchPoint = matchingLeftImage();

        drawLineInUnity(distance);
        if (leftAlready && rightAlready) // run once per 0.3 second
        {
            Point leftMatchingPoint = matchLeftImage(); 
            Point rightMatchingPoint = matchRightImagePointByLeftImagePoint(leftMatchingPoint);
            float disparityInPixel = Mathf.Abs(leftMatchingPoint.X - rightMatchingPoint.X);
            /*
            Debug.Log(leftPoint + ":" + rightPoint);
            Debug.Log(disparityInPixel);
            */
            //Debug.Log(disparityInPixel);
            distance = COEFF * FOCAL_LENGTH * BASELINE_LENGTH / disparityInPixel;
            this.leftAlready = false;
            this.rightAlready = false;
            //Debug.Log(parallexInPixel);
            //outputImageFile(leftImg, leftMatchingPoint, 1); // only 4 test
            //outputImageFile(rightImg, rightMatchingPoint, 2);
            //Debug.Log(distance+"?");
        }
    }

    private Point matchLeftImage()
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

    private Point matchRightImagePointByLeftImagePoint(Point p)
    {
        // parameter 'p': the center point of matching rectangle in left-image
        int height = p.Y;
        // Denote a=0,b=1,c=-1*height, ax + by + c = 0, which is eliline
        int topHeight = height - (template.Rows / 2);
        int buttomHeight = height + (template.Rows / 2);
        Mat rightImage_roi = rightImg.RowRange(topHeight-1, buttomHeight);

        Mat result = new Mat();
        double maxVal;
        double minVal;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        Cv2.MatchTemplate(rightImage_roi, template, result, TemplateMatchModes.SqDiffNormed);
        Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
        /*
        Cv2.Rectangle(rightImg, new Point(0, topHeight - 1), new Point(p.X * 2, buttomHeight), new Scalar(0, 255, 0), 2, LineTypes.Link8, 0);
        Cv2.Rectangle(rightImage_roi, minLoc, new Point(minLoc.X + template.Cols, minLoc.Y + template.Rows), new Scalar(0, 255, 0), 2, LineTypes.Link8, 0);
        Cv2.ImWrite(Application.persistentDataPath + "/right_roi.png", rightImg);
        Cv2.ImWrite(Application.persistentDataPath + "/right_img_roi.png", rightImage_roi);
        */
        return new Point(minLoc.X + template.Cols / 2, height); // center point of matcing rectangle
    }

    private void drawLineInUnity(float distance) // doesn't make any sense, except debug and test
    {
        Vector3 laserPosition = this.transform.position + new Vector3(0, 0, 1);
        Vector3 forwardDirection = this.transform.forward;
        Vector3 endPoint = laserPosition + forwardDirection * distance;
        Debug.DrawLine(laserPosition, endPoint);
    }

    private void outputImageFile(Mat image, Point center, int type)
    {
        // use for testing only
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

    public float getDistance()
    {
        return this.distance;
    }
}
