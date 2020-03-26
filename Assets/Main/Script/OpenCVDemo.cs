using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using OpenCvSharp;

public class OpenCVDemo : MonoBehaviour
{
    private MapBuilder mb;
    public InterfaceManager ui;

    private RRTNode root;
    private PowerfulEngine drone;
    // Start is called before the first frame update
    void Start()
    {
        test = true;
        mb = GameObject.FindGameObjectWithTag("Player").GetComponent<MapBuilder>();
        drone = GameObject.FindGameObjectWithTag("Player").GetComponent<PowerfulEngine>();
    }

    bool test;
    private void FixedUpdate()
    {
        /*
        if(ui.getGameMode() == InterfaceManager.MANUAL_MODE)
        {
            if (test)
            {
                root = new RRTNode(50, 75);
                RRTNode a = new RRTNode(50, 85, root);
                RRTNode b = new RRTNode(60, 85, a);
                RRTNode c = new RRTNode(60, 75, root);
                List<RRTNode> path = findPathOnRRT(root, b);
                path.Insert(0, root);
                drone.letDroneFlyByPath(path);
                test = false;
                return;
            }
            
            bool idle = drone.getIfIdle();
            if (idle)
            {
                root = new RRTNode(50, 75);
                RRTNode a = new RRTNode(50, 85, root);
                RRTNode b = new RRTNode(60, 85, a);
                RRTNode c = new RRTNode(60, 75, root);
                List<RRTNode> path = findPathOnRRT(b, c);
                path.Insert(0, b);
                drone.letDroneFlyByPath(path);
                test = false;
                return;
            }
        }
        */
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
