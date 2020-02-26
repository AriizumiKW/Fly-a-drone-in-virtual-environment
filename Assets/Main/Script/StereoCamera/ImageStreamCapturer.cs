using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System.Threading.Tasks;
using System.Threading;

public class ImageStreamCapturer : MonoBehaviour
{
    // this gameobject: StereoCamera-left or StereoCamera-right
    public const int LEFT_CAMERA = 1;
    public const int RIGHT_CAMERA = 2;
    public const int SCREEN_WIDTH = 756;
    public const int SCREEN_HEIGHT = 409;
    public const float FOCAL_LENGTH = 20.0f;

    public InterfaceManager uiManager;
    public DistanceCounter distanceCounter;

    private Camera thisStereoCamera;
    private int cameraType;
    private Mat capturedImage;
    // Start is called before the first frame update
    void Start()
    {
        thisStereoCamera = this.gameObject.GetComponent<Camera>();
        thisStereoCamera.focalLength = FOCAL_LENGTH; // by default, focal length = 20.0f (millimeters)
        string name = this.gameObject.name;
        if(name == "StereoCamera-left")
        {
            cameraType = LEFT_CAMERA;
        }
        else if(name == "StereoCamera-right")
        {
            cameraType = RIGHT_CAMERA;
        }
        else
        {
            Debug.LogError("ImageStreamCapturer.Camera_Type_Not_Found_Error");
        }
        //Texture2D t = ScreenCapture.CaptureScreenshotAsTexture();
        //Debug.Log(t.width + ":" + t.height);
        //Debug.Log(uiManager.getLock());
        StartCoroutine("imageCapturedThread");
        //Invoke("stopImageCapturedThread", 12);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        while(true)
        {
            if(uiManager.getLock() == false)
            {
                captureCameraImageAsMat();
            }
        }
        */
    }

    private IEnumerator imageCapturedThread() // actually, it is "Coroutine" (terminology in Unity), but behave as a similar way of thread
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            //Debug.Log(uiManager.getLock());
            if (!uiManager.getLock())
            {
                captureCameraImageAsMat();
                distanceCounter.countDistance();
            }
        }
    }

    private void stopImageCapturedThread()
    {
        //Debug.Log("Stop");
        StopCoroutine("imageCapturedThread");
    }

    private void captureCameraImageAsMat()
    {
        // capture image as Mat format, which can be directly used by OpenCvSharp
        RenderTexture captured = new RenderTexture(SCREEN_WIDTH, SCREEN_HEIGHT, 0);
        thisStereoCamera.targetTexture = captured;
        thisStereoCamera.Render();
        RenderTexture.active = captured;

        Texture2D image = new Texture2D(SCREEN_WIDTH, SCREEN_HEIGHT);
        image.ReadPixels(new UnityEngine.Rect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), 0, 0);
        image.Apply(); // apply pixel change

        // reset camera. It is for the next time when we need to capture camera images
        thisStereoCamera.targetTexture = null;
        RenderTexture.active = null;
        byte[] bytes = image.EncodeToPNG();
        //outputImageFile(image);
        // form an image, convert image from "Texture2D" type (A type of image in Unity) to "Mat" type (A type of image in OpenCvSharp)
        this.capturedImage = Mat.FromImageData(bytes);

        if (cameraType == LEFT_CAMERA) // send image to DistanceCounter
        {
            distanceCounter.setLeftImage(capturedImage);
        }
        else
        {
            distanceCounter.setRightImage(capturedImage);
        }
    }

    private void outputImageFile(Texture2D image) // use to test
    {
        string filename;
        if(cameraType == LEFT_CAMERA)
        {
            filename = Application.persistentDataPath + "/a left img.png";
        }
        else if(cameraType == RIGHT_CAMERA)
        {
            filename = Application.persistentDataPath + "/a right img.png";
        }
        else
        {
            filename = Application.persistentDataPath + "/no tag.png";
        }
        byte[] bytes = image.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);
    }

    public Mat getImage()
    {
        return this.capturedImage;
    }
}
