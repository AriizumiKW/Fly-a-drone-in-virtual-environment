using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeMaker : MonoBehaviour
{
    Transform[] children;
    LinkedList<Wall> walls = new LinkedList<Wall>();
    private int[,] connectMatrix = new int[63, 63]; 
    // connect matrix, show if two nodes connect with each other
    // =0, two nodes are not connected
    // =1, two nodes are connected
    // =2, two nodes are strong-connected (directly link to each other)

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

    const int FLAG_NO_CONSTRAINT = 0;
    const int FLAG_BAN_UP = 1;
    const int FLAG_BAN_DOWN = 2;
    const int FLAG_BAN_LEFT = 3;
    const int FLAG_BAN_RIGHT = 4; // to avoid infinite loop
    const int CONNECT = 1;
    const int STRONG_CONNECT = 2;
    private Boolean depthFirstSearch(int nodeOneRowNum, int nodeOneColumnNum, int nodeTwoRowNum, int nodeTwoColumnNum, int flag)
    {
        /*
         * DFS algorithm to search a tree, use recursion and dynamic programming
         * return true if two nodes are connected, else false 
         * formula of recursion and how dynamic programming : DFS(node1,node2) = DFS(node1's upward,node2) or DFS(node1's downward,node2) or
         *   DFS(node1's left,node2) or DFS(node1's right,node2)
        */
        //Debug.Log("Search:" + nodeOneRowNum + ":" + nodeOneColumnNum + "-" + nodeTwoRowNum + ":" + nodeTwoColumnNum +".."+flag);
        int nodeOneID = getNodeID(nodeOneRowNum, nodeOneColumnNum);
        int nodeTwoID = getNodeID(nodeTwoRowNum, nodeTwoColumnNum);
        if(connectMatrix[nodeOneID - 1,nodeTwoID - 1] != CONNECT && connectMatrix[nodeOneID - 1, nodeTwoID - 1] != STRONG_CONNECT)
        {
            int nodeOneAdjacentNodeID;
            
            if(nodeOneColumnNum != 9 && flag != FLAG_BAN_RIGHT)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum, nodeOneColumnNum + 1); // moving towards "right"
                if (connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == STRONG_CONNECT)
                { // right adjacent node
                    Boolean subResult = depthFirstSearch(nodeOneRowNum, nodeOneColumnNum + 1, nodeTwoRowNum, nodeTwoColumnNum, FLAG_BAN_LEFT);
                    if (subResult)
                    {
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = CONNECT; // dynamic programming
                        connectMatrix[nodeTwoID - 1, nodeOneAdjacentNodeID - 1] = CONNECT; // connect matrix should be symmetric
                        return true;
                    }
                }
            }
            if(nodeOneColumnNum != 1 && flag != FLAG_BAN_LEFT)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum, nodeOneColumnNum - 1); // moving towards"left"
                if (connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == STRONG_CONNECT)
                { // left adjacent node
                    Boolean subResult = depthFirstSearch(nodeOneRowNum, nodeOneColumnNum - 1, nodeTwoRowNum, nodeTwoColumnNum, FLAG_BAN_RIGHT);
                    if (subResult)
                    {
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = CONNECT; // dynamic programming
                        connectMatrix[nodeTwoID - 1, nodeOneAdjacentNodeID - 1] = CONNECT;
                        return true;
                    }
                }
            }
            if(nodeOneRowNum != 7 && flag != FLAG_BAN_DOWN)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum + 1, nodeOneColumnNum); // moving towards "down"
                if (connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == STRONG_CONNECT)
                { // down adjacent node
                    Boolean subResult = depthFirstSearch(nodeOneRowNum + 1, nodeOneColumnNum, nodeTwoRowNum, nodeTwoColumnNum, FLAG_BAN_UP);
                    if (subResult)
                    {
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = CONNECT; // dynamic programming
                        connectMatrix[nodeTwoID - 1, nodeOneAdjacentNodeID - 1] = CONNECT;
                        return true;
                    }
                }
            }
            if(nodeOneRowNum != 1 && flag != FLAG_BAN_UP)
            {
                nodeOneAdjacentNodeID = getNodeID(nodeOneRowNum - 1, nodeOneColumnNum); // moving towards "up"
                if (connectMatrix[nodeOneAdjacentNodeID - 1, nodeOneID - 1] == 2)
                { // up adjacent node
                    Boolean subResult = depthFirstSearch(nodeOneRowNum - 1, nodeOneColumnNum, nodeTwoRowNum, nodeTwoColumnNum, FLAG_BAN_DOWN);
                    if (subResult)
                    {
                        connectMatrix[nodeOneAdjacentNodeID - 1, nodeTwoID - 1] = CONNECT; // dynamic programming
                        connectMatrix[nodeTwoID - 1, nodeOneAdjacentNodeID - 1] = CONNECT;
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
            if (! depthFirstSearch(nodeOneRow, nodeOneColumn, nodeTwoRow, nodeTwoColumn, FLAG_NO_CONSTRAINT))
            {
                // if those two adjacent nodes has already connected, do nothing. Otherwise, break this wall
                aWall.breakThisWall();
                int nodeOneID = getNodeID(nodeOneRow, nodeOneColumn);
                int nodeTwoID = getNodeID(nodeTwoRow, nodeTwoColumn);
                connectMatrix[nodeOneID - 1, nodeTwoID - 1] = 2;
                connectMatrix[nodeTwoID - 1, nodeOneID - 1] = 2;
                //Debug.Log("Delete:" + nodeOneRow + ":" + nodeOneColumn + "-" + nodeTwoRow + ":" + nodeTwoColumn);
            }
        }
    }
}
