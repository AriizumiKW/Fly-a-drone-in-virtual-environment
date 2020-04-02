using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System.Threading.Tasks;

public class RRTDrawer : MonoBehaviour
{
    //public int num = 0;

    private Mat pic; // upper image layer
    private Mat checkedArea; // checkedArea info and drone trace. buttom image layer
    // x in scene: 25-475 (The width of pic: 450)
    // z in scene: 50-400 (The height of pic: 350)
    private Window myWin;
    public InterfaceManager uiManager;
    

    // Start is called before the first frame update
    void Awake()
    {
        pic = new Mat(350, 450, MatType.CV_8UC3, Scalar.LightGray); // image layer 1 (upper)
        checkedArea = new Mat(350, 450, MatType.CV_8UC3, Scalar.LightGray); // image layer 2 (buttom)
        myWin = new Window("RRT", pic);
        StartCoroutine("showGraph");
        /*
        int blue = checkedArea.At<Vec3b>(0, 0).Item0;
        int green = checkedArea.At<Vec3b>(0, 0).Item1;
        int red = checkedArea.At<Vec3b>(0, 0).Item2; // RGB value
        Debug.Log(blue + ":" + green + ":" + red);
        */
    }

    private IEnumerator showGraph() // Coroutine
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            Mat imgUsedToPrintOnScreen = mergeImageLayer(checkedArea, pic);
            if (!uiManager.getLock())
            {
                Cv2.ImShow("RRT", imgUsedToPrintOnScreen);
            }
        }
    }

    private IEnumerator waitForOneSecondsAndThanDiscard(Vector3 currPosition) // Coroutine
    {
        yield return new WaitForSeconds(1.0f);
        int x = (int)currPosition.x - 25;
        int y = 400 - (int)currPosition.z;
        Cv2.Circle(checkedArea, x, y, 2, Scalar.White, -4, LineTypes.Link8);
    }

    Mat result = new Mat(350, 450, MatType.CV_8UC3, Scalar.LightGray);
    private Mat mergeImageLayer(Mat buttomLayer, Mat upperLayer)
    {
        //Cv2.ImWrite(Application.persistentDataPath + "/buttom.png", buttomLayer);
        Cv2.CopyTo(upperLayer, result);
        Parallel.For(0, buttomLayer.Rows, row =>
        {
            for(int col=0; col<buttomLayer.Cols; col++)
            {
                int blue = buttomLayer.At<Vec3b>(row, col).Item0;
                int green = buttomLayer.At<Vec3b>(row, col).Item1;
                int red = buttomLayer.At<Vec3b>(row, col).Item2; // RGB value
                if(blue == 255 && green == 255 && red == 255) // white
                {
                    int blue2 = upperLayer.At<Vec3b>(row, col).Item0;
                    int green2 = upperLayer.At<Vec3b>(row, col).Item1;
                    int red2 = upperLayer.At<Vec3b>(row, col).Item2;
                    if(blue2 == 211 && green2 == 211 && red2 == 211) // light-grey
                    {
                        result.Set<Vec3b>(row, col, new Vec3b(255, 255, 255));
                    }
                }
                else if(blue == 144 && green == 238 && red == 144) // light-green
                {
                    int blue2 = upperLayer.At<Vec3b>(row, col).Item0;
                    int green2 = upperLayer.At<Vec3b>(row, col).Item1;
                    int red2 = upperLayer.At<Vec3b>(row, col).Item2;
                    result.Set<Vec3b>(row, col, new Vec3b(144, 238, 144));
                }
            }
        });
        return result;
    }

    public void drawTrace(Vector3 currPosition)
    {
        int x = (int)currPosition.x - 25;
        int y = 400 - (int)currPosition.z;
        Cv2.Circle(checkedArea, x, y, 2, Scalar.LightGreen, -4, LineTypes.Link8);
        StartCoroutine("waitForOneSecondsAndThanDiscard", currPosition);
    }

    public void addNode(RRTNode node)
    {
        int x = (int)node.X() - 25;
        int y = 400 - (int)node.Z();
        Cv2.Circle(pic, x, y, 2, Scalar.Blue, -1, LineTypes.Link8);
        int fatherX = (int)node.Father().X() - 25;
        int fatherZ = 400 - (int)node.Father().Z();
        Cv2.Line(pic, x, y, fatherX, fatherZ, Scalar.Green);
    }

    public void addRootNode(RRTNode node)
    {
        int x = (int)node.X() - 25;
        int y = 400 - (int)node.Z();
        Cv2.Circle(pic, x, y, 2, Scalar.Blue, -4, LineTypes.Link8);
    }

    public void drawWall(int x1, int y1, int x2, int y2)
    {
        Cv2.Line(pic, x1 - 25, 400 - y1, x2 - 25, 400 - y2, Scalar.Black);
    }

    public void drawObstacle(int x, int y)
    {
        Cv2.Circle(pic, x - 25, 400 - y, 3, Scalar.Brown, -1, LineTypes.Link8);
    }

    public void drawCheckedArea(Vector3 _ori, Vector3 _p1, Vector3 _p2)
    {
        List<List<Point>> listOfListOfPoints = new List<List<Point>>();
        List<Point> listOfPoints = new List<Point>();
        listOfPoints.Add(new Point((int)_ori.x - 25, 400 - (int)_ori.z));
        listOfPoints.Add(new Point((int)_p1.x - 25, 400 - (int)_p1.z));
        listOfPoints.Add(new Point((int)_p2.x - 25, 400 - (int)_p2.z));
        listOfListOfPoints.Add(listOfPoints);
        Cv2.FillPoly(checkedArea, listOfListOfPoints, Scalar.White, LineTypes.Link8);
    }

    public void drawCheckedArea(int x, int y)
    {
        Cv2.Circle(checkedArea, x - 25, 400 - y, 1, Scalar.White, -1, LineTypes.Link8);
    }

    public void drawPoint(int x, int y) // only for test
    {
        Cv2.Circle(pic, x - 25, 400 - y, 3, Scalar.Red, -1, LineTypes.Link8);
    }

    public void drawPoint(int x, int y, Scalar s, int radis) // only for test
    {
        Cv2.Circle(pic, x - 25, 400 - y, radis, s, -1, LineTypes.Link8);
    }

    public void drawLine(int x1, int y1, int x2, int y2, Scalar color) // only for test
    {
        Cv2.Line(pic, x1 - 25, 400 - y1, x2 - 25, 400 - y2, color);
    }
}
