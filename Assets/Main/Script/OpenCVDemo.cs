using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using OpenCvSharp;
using System.Threading.Tasks;

public class OpenCVDemo : MonoBehaviour
{
    private MapBuilder mb;
    public InterfaceManager ui;

    private RRTNode root;
    private PowerfulEngine drone;
    GridMap map;
    // Start is called before the first frame update
    void Start()
    {
        /*
        test = true;
        mb = GameObject.FindGameObjectWithTag("Player").GetComponent<MapBuilder>();
        drone = GameObject.FindGameObjectWithTag("Player").GetComponent<PowerfulEngine>();
        
        map = new GridMap();
        */

        Mat upper = new Mat(350, 450, MatType.CV_8UC3, Scalar.LightGray);
        Mat buttom = new Mat(350, 450, MatType.CV_8UC3, Scalar.LightGray);
        //Mat result = mergeImageLayer(buttom, upper);
        //Cv2.ImWrite(Application.persistentDataPath + "/gshsrttjj.png", result);
    }

    private Mat mergeImageLayer(Mat buttomLayer, Mat upperLayer)
    {
        Mat result = new Mat(350, 450, MatType.CV_8UC3, Scalar.LightGray);
        Cv2.CopyTo(upperLayer, result);
        for (int row = 0; row < buttomLayer.Rows; row++)
        {
            for (int col = 0; col < buttomLayer.Cols; col++)
            {
                Debug.Log(row + ": " + col);
                int blue = buttomLayer.At<Vec3b>(row, col).Item0;
                int green = buttomLayer.At<Vec3b>(row, col).Item1;
                int red = buttomLayer.At<Vec3b>(row, col).Item2; // RGB value
                if (blue == 255 && green == 255 && red == 255) // white
                {
                    int blue2 = upperLayer.At<Vec3b>(row, col).Item0;
                    int green2 = upperLayer.At<Vec3b>(row, col).Item1;
                    int red2 = upperLayer.At<Vec3b>(row, col).Item2;
                    if (blue2 == 211 && green2 == 211 && red2 == 211) // light-grey
                    {
                        result.Set<Vec3b>(row, col, new Vec3b(255, 255, 255));
                    }
                }
                else if (blue == 144 && green == 238 && red == 144) // light-green
                {
                    result.Set<Vec3b>(row, col, new Vec3b(144, 238, 144));
                }
            }
        }
        return result;
    }

    private List<RRTNode> findPathOnRRT(RRTNode curr, RRTNode dest) // curr: current position, dest: destination
    {
        if (curr == dest)
        {

            return new List<RRTNode>();
        }

        List<RRTNode> ancestors = new List<RRTNode>();
        RRTNode r = dest;
        bool b = true;
        ancestors.Add(r);
        while (b) // dont directly use "true", because visual studio will report an unreasonable error
        {
            if (r == root)
            {
                break;
            }
            r = r.Father();
            if (r == curr)
            {
                ancestors.Reverse();
                return ancestors;
            }
            ancestors.Add(r);
            
        }

        List<RRTNode> path = new List<RRTNode>();
        r = curr;
        while (b)
        {
            r = r.Father();
            path.Add(r);
            List<RRTNode> commonParentToDest = new List<RRTNode>();
            for (int i = 0; i < ancestors.Count; i++)
            {
                if (ancestors[i] == r)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        commonParentToDest.Add(ancestors[j]);
                    }
                    foreach (RRTNode node in commonParentToDest)
                    {
                        path.Add(node);
                    }
                    b = false; // dont directly use "break", because visual studio will report an unreasonable error
                }
                else
                {
                    commonParentToDest.Clear();
                }
            }
        }
        return path;
    }
}
