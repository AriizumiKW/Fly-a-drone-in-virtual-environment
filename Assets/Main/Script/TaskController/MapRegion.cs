using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRegion
{
    private Vector2 vertex1;
    private Vector2 vertex2;
    private Vector2 vertex3;

    public MapRegion(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        vertex1 = new Vector2(v1.x, v1.z);
        vertex2 = new Vector2(v2.x, v2.z);
        vertex3 = new Vector2(v3.x, v3.z);
    }

    public bool IfPointInRegion(Vector3 _point) // if point in trianglar region
    {
        // triangle: ABC: vertex1, vertex2, vertex3
        Vector3 A = new Vector3(vertex1.x, vertex1.y, 1);
        Vector3 B = new Vector3(vertex2.x, vertex2.y, 1);
        Vector3 C = new Vector3(vertex3.x, vertex3.y, 1);
        Vector3 point = new Vector3(_point.x, _point.z, 1);
        Vector3 PA = A - point;
        Vector3 PB = B - point;
        Vector3 PC = C - point;
        Vector3 AB = B - A;
        Vector3 BC = C - B;
        Vector3 CA = A - C;
        Vector3 v1;
        Vector3 v2;

        v1 = Vector3.Cross(PA, AB);
        v2 = Vector3.Cross(CA, AB);
        bool b = Vector3.Dot(v1, v2) >= 0;
        v1 = Vector3.Cross(PB, BC);
        v2 = Vector3.Cross(AB, BC);
        b = b && Vector3.Dot(v1, v2) >= 0;
        v1 = Vector3.Cross(PC, CA);
        v2 = Vector3.Cross(BC, CA);
        b = b && Vector3.Dot(v1, v2) >= 0;
        return b;
    }
}
