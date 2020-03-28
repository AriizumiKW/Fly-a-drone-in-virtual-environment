using System;
using System.Collections;
using System.Collections.Generic;

public class MapObstacle
{
    private float x;
    private float y;
    public const float radius = 10.0f;

    public MapObstacle(float _x, float _y)
    {
        this.x = _x;
        this.y = _y;
    }

    public float getDistanceFrom(float _x, float _y)
    {
        double distance = Math.Sqrt(Math.Pow(_x - x, 2) + Math.Pow(_y - y, 2));
        return (float)distance;
    }

    public float getDistanceFrom(MapObstacle o)
    {
        double distance = Math.Sqrt(Math.Pow(o.getX() - x, 2) + Math.Pow(o.getY() - y, 2));
        return (float)distance;
    }

    public float getX()
    {
        return this.x;
    }

    public float getY()
    {
        return this.y;
    }
}
