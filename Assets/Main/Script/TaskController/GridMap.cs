using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    public const int UNLABEL = 0;
    public const int CHECKED = 1;
    public const int CHECKED_AREA_EDGE = 2;
    public const int OBSTACLE = 3;
    public const int X_RANGE = 450; // x-coor: 25-475
    public const int Y_RANGE = 350; // y-coor: 50-400
    private int[,] map;
    private RRTDrawer demoGraph;

    public GridMap()
    {
        demoGraph = GameObject.FindGameObjectWithTag("Player").GetComponent<RRTDrawer>();
        map = new int[450, 350];
        for(int i = 10; i <= 40; i++)
        {
            for(int j = 10; j <= 40; j++)
            {
                map[i, j] = CHECKED;
                demoGraph.drawCheckedArea(i + 25, j + 50);
            }
        }
    }

    private void markAsEdge(float _x1, float _y1, float _x2, float _y2) // Bresenham Algorithm. Draw a line in a grid (mark as CHECKED_AREA_EDGE)
    {
        float x1 = Mathf.Round(_x1 - 25); // rounded to nearest integer
        float y1 = Mathf.Round(_y1 - 50);
        float x2 = Mathf.Round(_x2 - 25);
        float y2 = Mathf.Round(_y2 - 50);
        Vector2 start = new Vector2(x1, y1);
        Vector2 end = new Vector2(x2, y2);
        Vector2 direction;
        if (x1 == x2)
        {
            if(y2 >= y1)
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
            if(x2 >= x1)
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
        map[x, y] = CHECKED_AREA_EDGE; // mark start point before loop
        x = Mathf.RoundToInt(end.x);
        y = Mathf.RoundToInt(end.y);
        (x, y) = robustCheck(x, y);
        map[x, y] = CHECKED_AREA_EDGE; // mark end point before loop
        
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
            map[x, y] = CHECKED_AREA_EDGE;
            start += direction;
        }
    }

    private void fillTheTrianglarArea()
    {
        for(int i=0; i<X_RANGE; i++)
        {
            int buttomRow = -1; // impossible index: -1
            int upperRow = -1;
            for(int j=0; j<Y_RANGE; j++)
            {
                if(buttomRow != -1)
                {
                    if (map[i, j] == CHECKED_AREA_EDGE)
                    {
                        upperRow = j;
                    }
                }
                else
                {
                    if(map[i,j] == CHECKED_AREA_EDGE)
                    {
                        buttomRow = j;
                    }
                }
            }
            if(buttomRow != -1 && upperRow != -1) // pass two edge through this vertical line
            {
                for(int k=buttomRow; k<upperRow; k++)
                {
                    if(map[i, k] == UNLABEL)
                    {
                        map[i, k] = CHECKED;
                        demoGraph.drawCheckedArea(i + 25, k + 50);
                    }
                }
            }
            else if(buttomRow != -1 && upperRow == -1) // the vertex of trianglar
            {
                map[i, buttomRow] = CHECKED;
                demoGraph.drawCheckedArea(i + 25, buttomRow + 50);
            }
        }
    }

    private (int, int) robustCheck(int _x, int _y)
    {
        int x = _x;
        int y = _y;
        if (_x < 0)
        {
            Debug.Log("x: " + _x);
            x = 0;
        }
        else if (_x > X_RANGE - 1)
        {
            Debug.Log("x: " + _x);
            x = X_RANGE - 1;
        }

        if (_y < 0)
        {
            Debug.Log("y: " + _y);
            y = 0;
        }
        else if (_y > Y_RANGE - 1)
        {
            Debug.Log("y: " + _y);
            y = Y_RANGE - 1;
        }
        return (x, y);
    }

    public void setAnObstacle(float _x, float _y)
    {
        int x = Mathf.RoundToInt(_x - 25);
        int y = Mathf.RoundToInt(_y - 50);
        for(int i = x - 8; i < x + 8; i++)
        {
            for(int j = y - 8; j < y + 8; j++)
            {
                int col = i;
                int row = j;
                (col, row) = robustCheck(col, row);
                map[col, row] = OBSTACLE;
            }
        }
        demoGraph.drawObstacle(x + 25, y + 50);
    }

    public void setCheckedArea(Vector3 pointA, Vector3 pointB, Vector3 pointC) // all points in trianglar ABC, change to "CHECKED"
    {
        float ax = pointA.x;
        float ay = pointA.z;
        float bx = pointB.x;
        float by = pointB.z;
        float cx = pointC.x;
        float cy = pointC.z;
        markAsEdge(ax, ay, bx, by);
        markAsEdge(ax, ay, cx, cy);
        markAsEdge(bx, by, cx, cy);
        fillTheTrianglarArea();
    }

    public bool ifMeetAnObstacleAlongThisWay(float currX, float currZ, float destX, float destZ)
    {
        float x1 = Mathf.Round(currX - 25);
        float y1 = Mathf.Round(currZ - 50);
        float x2 = Mathf.Round(destX - 25);
        float y2 = Mathf.Round(destZ - 50);
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
        if (map[x, y] == OBSTACLE)
        {
            return true;
        }
        x = Mathf.RoundToInt(end.x);
        y = Mathf.RoundToInt(end.y);
        (x, y) = robustCheck(x, y);
        if (map[x, y] == OBSTACLE)
        {
            return true;
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
            if (map[x, y] == OBSTACLE)
            {
                return true;
            }
            
            start += direction;
        }
        return false;
    }

    public bool ifThisLineIsChecked(float _x1, float _y1, float _x2, float _y2)
    {
        float x1 = Mathf.Round(_x1 - 25);
        float y1 = Mathf.Round(_y1 - 50);
        float x2 = Mathf.Round(_x2 - 25);
        float y2 = Mathf.Round(_y2 - 50);
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
        if (map[x, y] == UNLABEL || map[x, y] == OBSTACLE)
        {
            return false;
        }
        x = Mathf.RoundToInt(end.x);
        y = Mathf.RoundToInt(end.y);
        (x, y) = robustCheck(x, y);
        if (map[x, y] == UNLABEL || map[x, y] == OBSTACLE)
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
            if (map[x, y] == UNLABEL || map[x, y] == OBSTACLE)
            {
                return false;
            }
            start += direction;
        }
        return true;
    }

    public bool ifThisPointIsChecked(float pointX, float pointZ)
    {
        int x = (int)(pointX - 25);
        int y = (int)(pointZ - 50);
        (x, y) = robustCheck(x, y);
        if (map[x, y] == CHECKED)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
