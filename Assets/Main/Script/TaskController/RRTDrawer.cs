using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;

public class RRTDrawer : MonoBehaviour
{
    //public int num = 0;

    private Mat pic;
    // x in scene: 25-475 (The width of pic: 450)
    // z in scene: 50-400 (The height of pic: 350)
    private Window myWin;
    public InterfaceManager uiManager;

    // Start is called before the first frame update
    void Awake()
    {
        pic = new Mat(350, 450, MatType.CV_8UC3, Scalar.White);
        myWin = new Window("RRT", pic);
        StartCoroutine("showGraph");
    }

    private void Update()
    {
        /*
        if(num != 0)
        {
            if(num <= 100)
            {
                addNode(new RRTNode(num, num, new RRTNode(200, 200)));
            }
            else
            {
                addNode(new RRTNode(num, num));
            }
            num = 0;
        }
        */
    }

    private IEnumerator showGraph() // Coroutine
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!uiManager.getLock())
            {
                Cv2.ImShow("RRT", pic);
            }
        }
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
        Cv2.Line(pic, x1 - 25, 400 - y1, x2 - 25, 400 - y2, Scalar.Gray);
    }

    public void drawObstacle(int x, int y)
    {
        Cv2.Circle(pic, x - 25, 400 - y, 5, Scalar.Brown, -1, LineTypes.Link8);
    }

    public void drawCheckedArea(Vector3 _ori, Vector3 _p1, Vector3 _p2)
    {
        List<List<Point>> listOfListOfPoints = new List<List<Point>>();
        List<Point> listOfPoints = new List<Point>();
        listOfPoints.Add(new Point((int)_ori.x - 25, 400 - (int)_ori.z));
        listOfPoints.Add(new Point((int)_p1.x - 25, 400 - (int)_p1.z));
        listOfPoints.Add(new Point((int)_p2.x - 25, 400 - (int)_p2.z));
        listOfListOfPoints.Add(listOfPoints);
        Cv2.FillPoly(pic, listOfListOfPoints, Scalar.LightGray, LineTypes.Link8);
    }

    public void drawPoint(int x, int y) // only for test
    {
        Cv2.Circle(pic, x - 25, 400 - y, 3, Scalar.Red, -1, LineTypes.Link8);
    }

    public void drawLine(int x1, int y1, int x2, int y2, Scalar color) // only for test
    {
        Cv2.Line(pic, x1 - 25, 400 - y1, x2 - 25, 400 - y2, color);
    }
}
