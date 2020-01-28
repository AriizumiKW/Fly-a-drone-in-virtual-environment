using System;
using UnityEngine;
public class Wall
{
    public const int TYPE_VERTICAL_WALL = 1;
    public const int TYPE_HORIZONTAL_WALL = 2;

    private int type; // =1 vertical wall, =2 horizontal wall
    private int coordX; // x coordinate
    private int coordZ; // z coordinate
    private GameObject thisWall;

    public Wall(int theType, float theCoordX, float theCoordZ, GameObject theWall)
    {
        type = theType;
        coordX = (int)theCoordX;
        coordZ = (int)theCoordZ;
        thisWall = theWall;
    }

    public Tuple<int,int,int,int> countAdjacentNodes()
    {
        // count those two adjacent nodes
        // the combination of the first and the second integer can be seen as the wall's ID number
        int columnNum;
        int rowNum;
        if(type == TYPE_HORIZONTAL_WALL)
        {
            columnNum = coordX / 50;
            rowNum = 8 - coordZ / 50;
            return new Tuple<int, int, int, int>(rowNum, columnNum, rowNum + 1, columnNum);
        }
        else // type == TYPE_VERTICAL_WALL
        {
            columnNum = (coordX - 25) / 50;
            rowNum = 8 - (coordZ - 25) / 50;
            return new Tuple<int, int, int, int>(rowNum, columnNum, rowNum, columnNum + 1);
        }
        // return: 1st & 2nd numbers are coordinate of the first adjacent node
        // 3rd & 4th numbers are the coordinate of the second adjacent node
    }

    public void breakThisWall()
    {
        thisWall.SetActive(false);
    }
}
