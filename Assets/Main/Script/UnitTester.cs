using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTester : MonoBehaviour
{
    public const int TYPE_VERTICAL_WALL = 1;
    public const int TYPE_HORIZONTAL_WALL = 2;
    // Start is called before the first frame update
    void Start()
    {
        //Tuple<int, int, int, int> t = countAdjacentNodes();
        //Debug.Log(t);
        float theta = Mathf.Atan(1.732f);
        Debug.Log(theta);
        float alpha = Mathf.Sin(theta);
        Debug.Log(alpha);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Tuple<int, int, int, int> countAdjacentNodes()
    {
        int type = TYPE_VERTICAL_WALL;
        int coordX = 175;
        int coordZ = 225;
        // count those two adjacent nodes
        int matrixColumnNum;
        int matrixRowNum;
        if (type == TYPE_HORIZONTAL_WALL)
        {
            matrixColumnNum = coordX / 50;
            matrixRowNum = 8 - coordZ / 50;
            return new Tuple<int, int, int, int>(matrixRowNum, matrixColumnNum, matrixRowNum + 1, matrixColumnNum);
        }
        else // type == TYPE_VERTICAL_WALL
        {
            matrixColumnNum = (coordX - 25) / 50;
            matrixRowNum = 8 - (coordZ - 25) / 50;
            return new Tuple<int, int, int, int>(matrixRowNum, matrixColumnNum, matrixRowNum, matrixColumnNum + 1);
        }
    }
}