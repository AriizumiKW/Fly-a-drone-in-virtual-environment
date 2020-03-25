using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using OpenCvSharp;

public class OpenCVDemo : MonoBehaviour
{
    private MapBuilder mb;

    private RRTNode root;
    // Start is called before the first frame update
    void Start()
    {
        /*
        mb = GameObject.FindGameObjectWithTag("Player").GetComponent<MapBuilder>();
        root = new RRTNode(50, 75);
        RRTNode a = new RRTNode(1, 1, root);
        RRTNode b = new RRTNode(2, 2, a);
        RRTNode c = new RRTNode(3, 3, root);
        List<RRTNode> path = findPathOnRRT(b, c);
        foreach(RRTNode node in path)
        {
            Debug.Log(node.X());
        }
        */
    }

    private void FixedUpdate()
    {
        //Vector3 unitDirection = new Vector3(0, 0, 6);
        //physics.MovePosition(physics.position + unitDirection * Time.deltaTime);
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
