using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeMaker : MonoBehaviour
{
    Transform[] children;
    LinkedList<Wall> walls = new LinkedList<Wall>();
    private int[,] connectMatrix = new int[63, 63]; // connect matrix, show if two nodes connect with each other

    // Start is called before the first frame update
    void Start()
    {
        children = GetComponentsInChildren<Transform>();
        resetMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int getNodeID(int rowNum, int columnNum)
    {
        return (rowNum - 1) * 9 + columnNum;
    }

    private Boolean depthFirstSearch(int nodeOneRowNum, int nodeOneColumnNum, int nodeTwoRowNum, int nodeTwoColumnNum)
    {
        // DFS algorithm to search a tree, use recursion and dynamic programming
        // return true if two nodes are connected, else false
        Debug.Log("Search:" + nodeOneRowNum + ":" + nodeOneColumnNum + "-" + nodeTwoRowNum + ":" + nodeTwoColumnNum);
        int nodeOneID = getNodeID(nodeOneRowNum, nodeOneColumnNum);
        int nodeTwoID = getNodeID(nodeTwoRowNum, nodeTwoColumnNum);
        if(connectMatrix[nodeOneID - 1,nodeTwoID - 1] != 1)
        {
            int nodeOneAdjacentNodeID;
            
            if(nodeOneColumnNum != 9)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum, nodeOneColumnNum + 1);
                if(connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == 1)
                {
                    Boolean subResult = depthFirstSearch(nodeOneRowNum, nodeOneColumnNum + 1, nodeTwoRowNum, nodeTwoColumnNum);
                    if (subResult)
                    {
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = 1; // dynamic programming
                        return true;
                    }
                }
            }
            if(nodeOneColumnNum != 1)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum, nodeOneColumnNum - 1);
                if (connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == 1)
                {
                    Boolean subResult = depthFirstSearch(nodeOneRowNum, nodeOneColumnNum - 1, nodeTwoRowNum, nodeTwoColumnNum);
                    if (subResult)
                    {
                        // dynamic programming
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = 1;
                        return true;
                    }
                }
            }
            if(nodeOneRowNum != 7)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum + 1, nodeOneColumnNum);
                if (connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == 1)
                {
                    Boolean subResult = depthFirstSearch(nodeOneRowNum + 1, nodeOneColumnNum, nodeTwoRowNum, nodeTwoColumnNum);
                    if (subResult)
                    {
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = 1; // dynamic programming
                        return true;
                    }
                }
            }
            if(nodeOneRowNum != 1)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum - 1, nodeOneColumnNum);
                if (connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == 1)
                {
                    Boolean subResult = depthFirstSearch(nodeOneRowNum - 1, nodeOneColumnNum, nodeTwoRowNum, nodeTwoColumnNum);
                    if (subResult)
                    {
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = 1; // dynamic programming
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public void resetMaze()
    {
        // spanning tree algorithm to make a random maze

        foreach (Transform child in children)
        {
            child.gameObject.SetActive(true);
        }

        walls.Clear();
        foreach (Transform child in children)
        {
            // initialize LinkedList
            string tag = child.gameObject.tag;
            if (tag == "horizontal-wall" || tag == "vertical-wall")
            {
                int type;
                float coordX = child.position.x;
                float coordZ = child.position.z;
                if (tag == "horizontal-wall")
                {
                    type = Wall.TYPE_HORIZONTAL_WALL;
                }
                else
                {
                    type = Wall.TYPE_VERTICAL_WALL;
                }
                Wall theWall = new Wall(type, coordX, coordZ, child.gameObject);
                walls.AddLast(theWall);
            }
        }

        while (walls.Count != 0)
        {
            // loop until every wall is considered
            int rand = UnityEngine.Random.Range(0, walls.Count);
            LinkedListNode<Wall> node = walls.First; // randomly choose a wall and try to break it
            for(int i = 0; i < rand; i++)
            {
                node = node.Next;
            }
            Wall aWall = node.Value;
            walls.Remove(node); // remove it from LinkedList
            Tuple<int, int, int, int> nodePairs = aWall.countAdjacentNodes();
            int nodeOneRow = nodePairs.Item1;
            int nodeOneColumn = nodePairs.Item2;
            int nodeTwoRow = nodePairs.Item3;
            int nodeTwoColumn = nodePairs.Item4;
            if (! depthFirstSearch(nodeOneRow, nodeOneColumn, nodeTwoRow, nodeTwoColumn))
            {
                // if those two adjacent nodes has already connected, do nothing. Otherwise, break this wall
                aWall.breakThisWall();
                int nodeOneID = getNodeID(nodeOneRow, nodeOneColumn);
                int nodeTwoID = getNodeID(nodeTwoRow, nodeTwoColumn);
                connectMatrix[nodeOneID - 1, nodeTwoID - 1] = 1;
                connectMatrix[nodeTwoID - 1, nodeOneID - 1] = 1;
                //Debug.Log("Delete:" + nodeOneRow + ":" + nodeOneColumn + "-" + nodeTwoRow + ":" + nodeTwoColumn);
            }
        }
    }
}
