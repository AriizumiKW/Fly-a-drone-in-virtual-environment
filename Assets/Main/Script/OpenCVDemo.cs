using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class OpenCVDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mat i = Cv2.ImRead(Application.persistentDataPath + "/pic.jpg");
        //Debug.Log(Application.persistentDataPath);
        //using (new Window("src image", i)) Cv2.WaitKey();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
