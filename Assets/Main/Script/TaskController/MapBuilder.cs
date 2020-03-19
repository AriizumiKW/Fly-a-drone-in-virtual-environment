using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public const float EFFECTIVE_DISTANCE = 60.0f;
    // store a map in memory
    private RRTDrawer demoGraph;
    private List<MapObstacle> obstacles;
    private List<MapRegion> checkedRegions;

    void Start()
    {
        obstacles = new List<MapObstacle>();
        checkedRegions = new List<MapRegion>();
        demoGraph = this.gameObject.GetComponent<RRTDrawer>();
    }

    private void setAnObstacle(Vector3 position)
    {
        float x = position.x;
        float y = position.z;
        obstacles.Add(new MapObstacle(x, y));
        demoGraph.drawObstacle((int)x, (int)y);
    }

    public void updateMap(float[] distances, float currOrientation, Vector3 currPosition)
    {
        for(int i = 1; i <= 7; i++)
        {
            float distance1 = distances[i - 1];
            float distance2 = distances[i];
            if(distance1 > EFFECTIVE_DISTANCE)
            {
                distance1 = EFFECTIVE_DISTANCE;
            }
            if(distance2 > EFFECTIVE_DISTANCE)
            {
                distance2 = EFFECTIVE_DISTANCE;
            }
            float angle1 = i * DistanceCounter.FIELD_OF_VIEW / 8 - (DistanceCounter.FIELD_OF_VIEW / 2) - (DistanceCounter.FIELD_OF_VIEW / 16);
            float angle2 = (i+1) * DistanceCounter.FIELD_OF_VIEW / 8 - (DistanceCounter.FIELD_OF_VIEW / 2) - (DistanceCounter.FIELD_OF_VIEW / 16);
            Vector3 direction1 = Quaternion.AngleAxis(angle1, new Vector3(0, 1, 0)) * this.transform.forward;
            Vector3 direction2 = Quaternion.AngleAxis(angle2, new Vector3(0, 1, 0)) * this.transform.forward;
            Vector3 vertex1 = currPosition + direction1 * distance1;
            Vector3 vertex2 = currPosition + direction2 * distance2;
            checkedRegions.Add(new MapRegion(currPosition, vertex1, vertex2));
            //demoGraph.drawCheckedArea(currPosition, vertex1, vertex2);
        }
        for (int i = 1; i <= 8; i++)
        {
            if(distances[i - 1] <= EFFECTIVE_DISTANCE)
            {
                float sightAngle = i * DistanceCounter.FIELD_OF_VIEW / 8 - (DistanceCounter.FIELD_OF_VIEW / 2) - (DistanceCounter.FIELD_OF_VIEW / 16);
                Vector3 direction = Quaternion.AngleAxis(sightAngle, new Vector3(0, 1, 0)) * this.transform.forward;
                Vector3 endPoint = currPosition + direction * distances[i - 1];
                setAnObstacle(endPoint);
            }
        }
    }

    public void resetMap()
    {
        obstacles.Clear();
    }

    public bool ifMeetObstacleAlongTheDirection(float oriX, float oriY, float endX, float endY)
    {
        float x1_minus_x2 = oriX - endX;
        float y1_minus_y2 = oriY - endY;
        float a = -1 * y1_minus_y2;
        float b = x1_minus_x2;
        float c = (oriX * y1_minus_y2) - (oriY * x1_minus_x2); // ax+by+c=0, pass (oriX,oriY) and (endX,endY)
        foreach(MapObstacle obstacle in obstacles)
        {
            float x = obstacle.getX();
            float y = obstacle.getY();
            float distance = Mathf.Abs(a * x + b * y + c) / Mathf.Sqrt(a * a + b * b);
            if(distance <= MapObstacle.radius)
            {
                return true;
            }
        }
        return false;
    }

    public bool ifTargetPointIsChecked(Vector3 _point)
    {
        foreach(MapRegion region in checkedRegions)
        {
            if (region.IfPointInRegion(_point))
            {
                return true;
            }
        }
        return false;
    }
}
