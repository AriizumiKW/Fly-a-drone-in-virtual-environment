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

    private (int, int) robustCheck(int _x, int _y) // 0-450,0-350, only for test
    {
        int x = _x;
        int y = _y;
        if (_x < 0)
        {
            //Debug.Log("x: " + _x);
            x = 0;
        }
        else if (_x > 449)
        {
            //Debug.Log("x: " + _x);
            x = 449;
        }

        if (_y < 0)
        {
            //Debug.Log("y: " + _y);
            y = 0;
        }
        else if (_y > 349)
        {
            //Debug.Log("y: " + _y);
            y = 349;
        }
        return (x, y);
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
    public void drawPossibleObstacle(int x, int y)
    {
        Cv2.Circle(pic, x - 25, 400 - y, 3, Scalar.Red, -1, LineTypes.Link8);
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

    public bool checkWall(float _x1, float _y1, float _x2, float _y2) // for test
    {
        float x1 = Mathf.Round(_x1 - 25);
        float y1 = Mathf.Round(400 - _y1);
        float x2 = Mathf.Round(_x2 - 25);
        float y2 = Mathf.Round(400 - _y2 );
        Vector2 start = new Vector2(x1, y1);
        Vector2 end = new Vector2(x2, y2);
        Vector2 direction;
        if (x1 == x2)
        {
            if (y2 >= y1)
            {
                direction = new Vector2(0, 1.0f);
            }
            else
            {
                direction = new Vector2(0, -1.0f);
            }
        }
        else
        {
            if (x2 >= x1)
            {
                direction = new Vector2(1.0f, (y2 - y1) / (x2 - x1));
            }
            else
            {
                direction = new Vector2(-1.0f, (y2 - y1) / (x1 - x2));
            }
        }

        int x = Mathf.RoundToInt(start.x);
        int y = Mathf.RoundToInt(start.y);
        (x, y) = robustCheck(x, y);
        int blue = pic.At<Vec3b>(x, y).Item0;
        int green = pic.At<Vec3b>(x, y).Item1;
        int red = pic.At<Vec3b>(x, y).Item2;
        if (blue == 0 && green == 0 && red == 0)
        {
            return false;
        }
        x = Mathf.RoundToInt(end.x);
        y = Mathf.RoundToInt(end.y);
        (x, y) = robustCheck(x, y);
        blue = pic.At<Vec3b>(x, y).Item0;
        green = pic.At<Vec3b>(x, y).Item1;
        red = pic.At<Vec3b>(x, y).Item2;
        if (blue == 0 && green == 0 && red == 0)
        {
            return false;
        }

        while (true)
        {
            if (y2 >= y1)
            {
                if (start.y >= end.y)
                {
                    break;
                }
            }
            else
            {
                if (start.y <= end.y)
                {
                    break;
                }
            }
            x = Mathf.RoundToInt(start.x);
            y = Mathf.RoundToInt(start.y);
            (x, y) = robustCheck(x, y);
            blue = pic.At<Vec3b>(x, y).Item0;
            green = pic.At<Vec3b>(x, y).Item1;
            red = pic.At<Vec3b>(x, y).Item2;
            if (blue == 0 && green == 0 && red == 0)
            {
                return false;
            }
            for (float k = start.y; k <= (start + direction).y; k += 1.0f)
            {
                int x0 = Mathf.RoundToInt(start.x);
                int y0 = Mathf.RoundToInt(k);
                (x0, y0) = robustCheck(x0, y0);
                blue = pic.At<Vec3b>(x0, y0).Item0;
                green = pic.At<Vec3b>(x0, y0).Item1;
                red = pic.At<Vec3b>(x0, y0).Item2;
                if (blue == 0 && green == 0 && red == 0)
                {
                    return false;
                }
            }
            start += direction;
        }
        return true;
    }

}
