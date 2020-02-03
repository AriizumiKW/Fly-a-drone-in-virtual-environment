using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class OpenCVDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*
        Mat red = Cv2.ImRead(Application.persistentDataPath + "/template.png");
        Mat wolf = Cv2.ImRead(Application.persistentDataPath + "/pic.jpg");
        //Debug.Log(Application.persistentDataPath);
        Mat result = new Mat();
        double maxVal;
        double minVal;
        Point minLoc = new Point();
        Point maxLoc = new Point();
        Debug.Log("stop1");
        Cv2.MatchTemplate(wolf, red, result, TemplateMatchModes.SqDiffNormed);
        Debug.Log("stop2");
        Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);
        Debug.Log("stop3");
        Debug.Log(minLoc);
        using (new Window("image", leftImg))
        {
            Cv2.WaitKey();
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
