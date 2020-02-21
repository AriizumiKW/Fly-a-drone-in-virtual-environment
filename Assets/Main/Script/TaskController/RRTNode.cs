using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRTNode
{
    // Rapidly Exploring Random Tree Node
    private float nodeID;
    private float x;
    private float z;
    private RRTNode father;
    private List<RRTNode> children;
    private float distanceToFather;

    public RRTNode(float theX, float theZ)
    {
        // node without father, that is, the root
        x = theX;
        z = theZ;
        distanceToFather = 0;
        nodeID = z * 500 + x;
    }
    
    public RRTNode(float theX, float theZ, RRTNode theFather)
    {
        x = theX;
        z = theZ;
        father = theFather;
        distanceToFather = Mathf.Sqrt(Mathf.Pow(father.X() - this.x, 2) + Mathf.Pow(father.Z() - this.z, 2));
        nodeID = z * 500 + x;
    }

    public float ID()
    {
        return nodeID;
    }

    public float X()
    {
        return x;
    }

    public float Z()
    {
        return z;
    }

    public float getDistanceToFather()
    {
        return this.distanceToFather;
    }

    public RRTNode Father()
    {
        return this.father;
    }

    public float distanceTo(RRTNode other)
    {
        float distanceSquare = Mathf.Pow(other.X() - x, 2) + Mathf.Pow(other.Z() - z, 2);
        return Mathf.Sqrt(distanceSquare);
    }

    public float distanceTo(float otherX, float otherZ)
    {
        float distanceSquare = Mathf.Pow(otherX - x, 2) + Mathf.Pow(otherZ - z, 2);
        return Mathf.Sqrt(distanceSquare);
    }

    public List<RRTNode> Children()
    {
        return this.children;
    }

    public void addChild(RRTNode child)
    {
        children.Add(child);
    }
}
