using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    public const int UNLABEL = 0;
    public const int CHECKED = 1;
    //public const int POSSIBLE_CHECKED = 2;
    public const int CHECKED_AREA_EDGE = 3;
    public const int OBSTACLE = 4;
    public const int POSSIBLE_OBSTACLE = 5; // probably there is an obstacle, but the belief is less than the case that we can ensure
    /*
     * We need to double check a grid if it is checked or if it is an obstacle.
     * It is to avoid outliers as far as possible.
     * 
     * e.g.
     * DistanceCounter find there is a wall. Then mark it with "5".
     * If DistanceCounter find there is a wall twice, Then mark it with "4". We ensure it isnt an outlier.
     */
    public const int X_RANGE = 450; // x-coor: 25-475
    public const int Y_RANGE = 350; // y-coor: 50-400
    private int[,] map;
    private RRTDrawer demoGraph;

    public GridMap()
    {
        demoGraph = GameObject.FindGameObjectWithTag("Player").GetComponent<RRTDrawer>();
        map = new int[450, 350];
        for(int i = 5; i <= 45; i++)
        {
            for(int j = 5; j <= 45; j++)
            {
                map[i, j] = CHECKED;
                demoGraph.drawCheckedArea(i + 25, j + 50);
            }
        }
    }

    private void markAsEdge(float _x1, float _y1, float _x2, float _y2, int[,] mapCopy) // Bresenham Algorithm. Draw a line in a grid (mark as CHECKED_AREA_EDGE)
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
        mapCopy[x, y] = CHECKED_AREA_EDGE; // mark start point before loop
        x = Mathf.RoundToInt(end.x);
        y = Mathf.RoundToInt(end.y);
        (x, y) = robustCheck(x, y);
        mapCopy[x, y] = CHECKED_AREA_EDGE; // mark end point before loop
        
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
            mapCopy[x, y] = CHECKED_AREA_EDGE;
            //demoGraph.drawPoint(x + 25, y + 50, OpenCvSharp.Scalar.Black, 1);
            start += direction;
        }
    }

    private void fillTheTrianglarArea(int[,] mapCopy)
    {
        for(int i=0; i<X_RANGE; i++)
        {
            int buttomRow = -1; // impossible index: -1
            int upperRow = -1;
            for(int j=0; j<Y_RANGE; j++)
            {
                if(buttomRow != -1)
                {
                    if (mapCopy[i, j] == CHECKED_AREA_EDGE)
                    {
                        upperRow = j;
                    }
                }
                else
                {
                    if(mapCopy[i,j] == CHECKED_AREA_EDGE)
                    {
                        buttomRow = j;
                    }
                }
            }
            if(buttomRow != -1 && upperRow != -1) // pass two edge through this vertical line
            {
                for(int k=buttomRow; k<=upperRow; k++)
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
                if (map[i, buttomRow] == UNLABEL)
                {
                    map[i, buttomRow] = CHECKED;
                    demoGraph.drawCheckedArea(i + 25, buttomRow + 50);
                }
            }
        }
    }

    private (int, int) robustCheck(int _x, int _y)
    {
        int x = _x;
        int y = _y;
        if (_x < 0)
        {
            //Debug.Log("x: " + _x);
            x = 0;
        }
        else if (_x > X_RANGE - 1)
        {
            //Debug.Log("x: " + _x);
            x = X_RANGE - 1;
        }

        if (_y < 0)
        {
            //Debug.Log("y: " + _y);
            y = 0;
        }
        else if (_y > Y_RANGE - 1)
        {
            //Debug.Log("y: " + _y);
            y = Y_RANGE - 1;
        }
        return (x, y);
    }

    public void setAnObstacle(float _x, float _y)
    {
        int x = Mathf.RoundToInt(_x - 25);
        int y = Mathf.RoundToInt(_y - 50);
        for(int i = x - OBSTACLE_radius; i < x + OBSTACLE_radius; i++)
        {
            for(int j = y - OBSTACLE_radius; j < y + OBSTACLE_radius; j++)
            {
                int col = i;
                int row = j;
                (col, row) = robustCheck(col, row);
                map[col, row] = OBSTACLE;
            }
        }
        (x, y) = robustCheck(x, y);
        if(map[x, y] != CHECKED && map[x + 4, y] != CHECKED && map[x, y + 4] != CHECKED)
        {
            demoGraph.drawObstacle(x + 25, y + 50);
        }
    }

    public const int TEMP_checked_before = 6;
    //public const int TEMP_possible_checked_before = 7;
    public const int TEMP_unlabel_before = 8;
    public const int TEMP_obstacle_before = 9; 
    public const int TEMP_possible_obstacle_before = 10; // used to recover the state before
    public const int OBSTACLE_radius = 4;

    public List<Vector3> setObstacles(List<Vector3> obstacles) // outlier filter
    {
        List<Vector3> inliers = new List<Vector3>();
        foreach (Vector3 obstacle in obstacles)
        {
            int x = Mathf.RoundToInt(obstacle.x - 25);
            int y = Mathf.RoundToInt(obstacle.z - 50);
            for(int i=x-OBSTACLE_radius; i<x+OBSTACLE_radius; i++)
            {
                for(int j=y-OBSTACLE_radius; j<y+OBSTACLE_radius; j++)
                {
                    int col = i;
                    int row = j;
                    (col, row) = robustCheck(col, row);
                    if (map[col, row] == UNLABEL)
                    {
                        map[col, row] = TEMP_unlabel_before;
                    }
                    else if (map[col, row] == OBSTACLE)
                    {
                        map[col, row] = TEMP_obstacle_before;
                    }
                    else if (map[col, row] == POSSIBLE_OBSTACLE)
                    {
                        map[col, row] = TEMP_possible_obstacle_before;
                    }
                    else if (map[col, row] == CHECKED)
                    {
                        map[col, row] = TEMP_checked_before;
                    }
                }
            }
        }

        foreach (Vector3 obstacle in obstacles)
        {
            int x = Mathf.RoundToInt(obstacle.x - 25);
            int y = Mathf.RoundToInt(obstacle.z - 50);
            int belief_itIsObstacle = 0;
            for (int i = x - 12; i <= x + 12; i++)
            {
                for (int j = y - 12; j <= y + 12; j++)
                {
                    if (i >= 0 && j >= 0 && i <= X_RANGE - 1 && j <= Y_RANGE - 1)
                    {
                        if (map[i, j] == OBSTACLE || map[i, j] == TEMP_unlabel_before || map[i, j] == TEMP_obstacle_before || 
                            map[i, j] == TEMP_possible_obstacle_before || map[i, j] == TEMP_checked_before)
                        {
                            belief_itIsObstacle += 1;
                        }
                    }
                    //demoGraph.drawPoint(i + 25, j + 50, OpenCvSharp.Scalar.LightGreen, 1);
                }
            }
            if (belief_itIsObstacle >= 120) // more than 180 grids are obstacle in neigbour, so it is an inlier
            {
                //Debug.Log(belief_itIsObstacle);
                inliers.Add(obstacle);
            }
        }

        foreach (Vector3 obstacle in obstacles)
        {
            int x = Mathf.RoundToInt(obstacle.x - 25);
            int y = Mathf.RoundToInt(obstacle.z - 50);
            for (int i = x - OBSTACLE_radius; i < x + OBSTACLE_radius; i++)
            {
                for (int j = y - OBSTACLE_radius; j < y + OBSTACLE_radius; j++)
                {
                    int col = i;
                    int row = j;
                    (col, row) = robustCheck(col, row);
                    if (map[col, row] == TEMP_unlabel_before)
                    {
                        map[col, row] = UNLABEL;
                    }
                    else if (map[col, row] == TEMP_obstacle_before)
                    {
                        map[col, row] = OBSTACLE;
                    }
                    else if (map[col, row] == TEMP_possible_obstacle_before)
                    {
                        map[col, row] = POSSIBLE_OBSTACLE;
                    }
                    else if (map[col, row] == TEMP_checked_before)
                    {
                        map[col, row] = CHECKED;
                    }
                }
            }
        }

        foreach (Vector3 inlier in inliers)
        {
            int x = Mathf.RoundToInt(inlier.x - 25);
            int y = Mathf.RoundToInt(inlier.z - 50);
            for (int i = x - OBSTACLE_radius; i < x + OBSTACLE_radius; i++)
            {
                for (int j = y - OBSTACLE_radius; j < y + OBSTACLE_radius; j++)
                {
                    int col = i;
                    int row = j;
                    (col, row) = robustCheck(col, row);
                    if(map[col, row] == POSSIBLE_OBSTACLE)
                    {
                        map[col, row] = OBSTACLE;
                        demoGraph.drawObstacle(x + 25, y + 50);
                    }
                    else
                    {
                        map[col, row] = POSSIBLE_OBSTACLE;
                        //demoGraph.drawPossibleObstacle(x + 25, y + 50);
                    }
                }
            }
        }
        return inliers;
    }

    public void setCheckedArea(Vector3 pointA, Vector3 pointB, Vector3 pointC) // all points in trianglar ABC, change to "CHECKED"
    {
        float ax = pointA.x;
        float ay = pointA.z;
        float bx = pointB.x;
        float by = pointB.z;
        float cx = pointC.x;
        float cy = pointC.z;
        int[,] mapCopy = (int[,])map.Clone();
        markAsEdge(ax, ay, bx, by, mapCopy);
        markAsEdge(ax, ay, cx, cy, mapCopy);
        markAsEdge(bx, by, cx, cy, mapCopy);
        fillTheTrianglarArea(mapCopy);
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
        if (map[x, y] == OBSTACLE || map[x, y] == POSSIBLE_OBSTACLE)
        {
            return true;
        }
        x = Mathf.RoundToInt(end.x);
        y = Mathf.RoundToInt(end.y);
        (x, y) = robustCheck(x, y);
        if (map[x, y] == OBSTACLE || map[x, y] == POSSIBLE_OBSTACLE)
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
            if (map[x, y] == OBSTACLE || map[x, y] == POSSIBLE_OBSTACLE)
            {
                return true;
            }
            
            start += direction;
        }
        if (demoGraph.checkWall(x1 + 25, y1 + 50, x2 + 25, y2 + 50) == false) // for test
        {
            return false;
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
        if (map[x, y] == UNLABEL || map[x, y] == OBSTACLE || map[x, y] == POSSIBLE_OBSTACLE)
        {
            return false;
        }
        x = Mathf.RoundToInt(end.x);
        y = Mathf.RoundToInt(end.y);
        (x, y) = robustCheck(x, y);
        if (map[x, y] == UNLABEL || map[x, y] == OBSTACLE || map[x, y] == POSSIBLE_OBSTACLE)
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
            if (map[x, y] == UNLABEL || map[x, y] == OBSTACLE || map[x, y] == POSSIBLE_OBSTACLE)
            {
                return false;
            }
            for(float k=start.y; k<=(start+direction).y; k += 1.0f)
            {
                int x0 = Mathf.RoundToInt(start.x);
                int y0 = Mathf.RoundToInt(k);
                (x0, y0) = robustCheck(x0, y0);
                if (map[x0, y0] == UNLABEL || map[x0, y0] == OBSTACLE || map[x0, y0] == POSSIBLE_OBSTACLE)
                {
                    return false;
                }
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
