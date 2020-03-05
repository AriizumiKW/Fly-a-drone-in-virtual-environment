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
    private Mat leftImg; // 756 * 409 pixels
    private Mat rightImg; // 756 * 409 pixels
    private Mat fundMatrix;
    private Mat template;

    // Start is called before the first frame update
    void Awake()
    {
        timer = 0;
        //template = Mat.FromImageData(templateTexture2DFormat.EncodeToPNG()); // convert to OpenCv.Mat data type
        template = Cv2.ImRead(Application.persistentDataPath + "/template.png");
    }

    void Start()
    {
        initFundMatrix();
    }

    // Update is called once per frame
    public void countDistance()
    {
        if (uiManager.getLock()) // if locked, do nothing
        {
            return;
        }
        //Debug.Log("kkkk");
        // count the distance
        //Point matchPoint = matchingLeftImage();

        //drawLineInUnity(distance);
        if (leftAlready && rightAlready) // 新版距离计算
        {
            leftAlready = false;
            rightAlready = false;
            //cutLeftImageToEightParts();
            //Debug.Log("111211");
        }
        /*
        if (leftAlready && rightAlready) // 旧版距离计算
        {
            Point leftMatchingPoint = matchLeftImage(); 
            Point rightMatchingPoint = matchRightImagePointByLeftImagePoint(leftMatchingPoint);
            float disparityInPixel = Mathf.Abs(leftMatchingPoint.X - rightMatchingPoint.X);
            //Debug.Log(disparityInPixel);
            distance = COEFF * FOCAL_LENGTH * BASELINE_LENGTH / disparityInPixel;
            this.leftAlready = false;
            this.rightAlready = false;
            //Debug.Log(parallexInPixel);
            //outputImageFile(leftImg, leftMatchingPoint, 1); // only 4 test
            //outputImageFile(rightImg, rightMatchingPoint, 2);
            //Debug.Log(distance+"?");
        }
        */
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
        part1 = leftImg_roi.ColRange(0, (int)(0.125 * leftImg.Cols));
        part2 = leftImg_roi.ColRange((int)(0.125 * leftImg.Cols), (int)(0.25 * leftImg.Cols));
        part3 = leftImg_roi.ColRange((int)(0.25 * leftImg.Cols), (int)(0.375 * leftImg.Cols));
        part4 = leftImg_roi.ColRange((int)(0.375 * leftImg.Cols), (int)(0.5 * leftImg.Cols));
        part5 = leftImg_roi.ColRange((int)(0.5 * leftImg.Cols), (int)(0.625 * leftImg.Cols));
        part6 = leftImg_roi.ColRange((int)(0.625 * leftImg.Cols), (int)(0.75 * leftImg.Cols));
        part7 = leftImg_roi.ColRange((int)(0.75 * leftImg.Cols), (int)(0.875 * leftImg.Cols));
        part8 = leftImg_roi.ColRange((int)(0.875 * leftImg.Cols), leftImg.Cols);
        part1_centre = new Mat();
        part2_centre = new Mat();
        part3_centre = new Mat();
        part4_centre = new Mat();
        part5_centre = new Mat();
        part6_centre = new Mat();
        part7_centre = new Mat();
        part8_centre = new Mat();
        part1_centre.Add(new Point3d((int)(0.0625 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part2_centre.Add(new Point3d((int)(0.1875 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part3_centre.Add(new Point3d((int)(0.3125 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part4_centre.Add(new Point3d((int)(0.4375 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part5_centre.Add(new Point3d((int)(0.5625 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part6_centre.Add(new Point3d((int)(0.6875 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part7_centre.Add(new Point3d((int)(0.8125 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
        part8_centre.Add(new Point3d((int)(0.9375 * leftImg.Cols), (int)(leftImg.Height / 2), 1));
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
        //findDisparity(part1, part1_epliline);
    }

    private void findDisparity(Mat templateFromLeftImg, Mat epliline)
    {
        Mat rightImg_roi = new Mat(templateFromLeftImg.Rows, 756, MatType.CV_8UC3);
        double a = epliline.At<Point3d>(0).X;
        double b = epliline.At<Point3d>(0).Y;
        double c = epliline.At<Point3d>(0).Z; // ax+by+cz=0 (homogeneous coordinates)
        for (int col = 0; col < 756; col++) // affine transformation
        {
            double y = (-c - a * col)/ b;
            for (int row = (int)(y - rightImg_roi.Rows/2); row < (int)(y + rightImg_roi.Rows/2); row++)
            {
                rightImg_roi.Set<Vec3b>(row - (int)(y - rightImg_roi.Rows / 2), col, rightImg.Get<Vec3b>(row, col));
            }
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
