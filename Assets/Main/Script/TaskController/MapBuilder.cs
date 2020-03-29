using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public const float EFFECTIVE_DISTANCE = 60.0f;
    // store a map in memory
    private RRTDrawer demoGraph;
    private GridMap map;

    void Start()
    {
        map = new GridMap();
        demoGraph = this.gameObject.GetComponent<RRTDrawer>();
    }

    private void setAnObstacle(Vector3 position)
    {
        float x = position.x;
        float y = position.z;
        map.setAnObstacle(x, y);
    }

    private List<Vector3> smoothObstacles(List<Vector3> _endPoints)
    {
        if(_endPoints.Count <= 1)
        {
            return _endPoints;
        }
        float y_variance = 0;
        float x_variance = 0;
        float x_max = _endPoints[0].x;
        float x_min = _endPoints[0].x;
        float y_max = _endPoints[0].z;
        float y_min = _endPoints[0].z;
        float x_mean = 0;
        float y_mean = 0;
        int counter = 0; // how many points are vaild

        foreach(Vector3 point in _endPoints)
        {
            if(point.x == DistanceCounter.INVALID_DISTANCE || point.z == DistanceCounter.INVALID_DISTANCE)
            {
                continue;
            }
            x_mean += point.x;
            y_mean += point.z;
            counter++;
            if(point.x > x_max)
            {
                x_max = point.x;
            }
            else if(point.x < x_min)
            {
                x_min = point.x;
            }
            if(point.z > y_max)
            {
                y_max = point.z;
            }
            else if(point.z < y_min)
            {
                y_min = point.z;
            }
        }
        x_mean /= counter;
        y_mean /= counter;
        foreach(Vector3 point in _endPoints)
        {
            if (point.x == DistanceCounter.INVALID_DISTANCE || point.z == DistanceCounter.INVALID_DISTANCE)
            {
                continue;
            }
            x_variance += Mathf.Pow(point.x - x_mean, 2);
            y_variance += Mathf.Pow(point.z - y_mean, 2);
        }
        x_variance -= Mathf.Pow(x_max - x_mean, 2); // get rid of max and min while counting variance
        x_variance -= Mathf.Pow(x_min - x_mean, 2);
        y_variance -= Mathf.Pow(y_max - y_mean, 2);
        y_variance -= Mathf.Pow(y_min - y_mean, 2);

        List<Vector3> pointsAfterSmooth = new List<Vector3>(); 
        if (x_variance > y_variance) // it should be horizontal wall
        {
            foreach(Vector3 point in _endPoints)
            {
                if (point.x == DistanceCounter.INVALID_DISTANCE || point.z == DistanceCounter.INVALID_DISTANCE)
                {
                    continue;
                }
                pointsAfterSmooth.Add(new Vector3(point.x, 0, y_mean));
            }
        }
        else // it should be vertical wall
        {
            foreach (Vector3 point in _endPoints)
            {
                if (point.x == DistanceCounter.INVALID_DISTANCE || point.z == DistanceCounter.INVALID_DISTANCE)
                {
                    continue;
                }
                pointsAfterSmooth.Add(new Vector3(x_mean, 0, point.z));
            }
        }
        return pointsAfterSmooth;
    }

    public void updateMap(float[] distances, float currOrientation, Vector3 currPosition)
    {
        for (int i = 1; i <= 7; i++)
        {
            float distance1 = distances[i - 1];
            float distance2 = distances[i];
            if(distance1 == DistanceCounter.INVALID_DISTANCE || distance2 == DistanceCounter.INVALID_DISTANCE)
            {
                return;
            }

            if(distance1 > EFFECTIVE_DISTANCE)
            {
                distance1 = EFFECTIVE_DISTANCE;
            }
            float angle1 = i * DistanceCounter.FIELD_OF_VIEW / 8 - (DistanceCounter.FIELD_OF_VIEW / 2) - (DistanceCounter.FIELD_OF_VIEW / 16);
            Vector3 direction1 = Quaternion.AngleAxis(angle1, new Vector3(0, 1, 0)) * this.transform.forward;
            Vector3 vertex1 = currPosition + direction1 * distance1;

            if(distance2 > EFFECTIVE_DISTANCE)
            {
                distance2 = EFFECTIVE_DISTANCE;
            }
            float angle2 = (i+1) * DistanceCounter.FIELD_OF_VIEW / 8 - (DistanceCounter.FIELD_OF_VIEW / 2) - (DistanceCounter.FIELD_OF_VIEW / 16);
            Vector3 direction2 = Quaternion.AngleAxis(angle2, new Vector3(0, 1, 0)) * this.transform.forward;
            Vector3 vertex2 = currPosition + direction2 * distance2;
            map.setCheckedArea(currPosition, vertex1, vertex2);
            //demoGraph.drawCheckedArea(currPosition, vertex1, vertex2);
        }

        List<Vector3> endPoints = new List<Vector3>();
        for (int i = 1; i <= 8; i++)
        {
            if (distances[i - 1] <= EFFECTIVE_DISTANCE)
            {
                float sightAngle = i * DistanceCounter.FIELD_OF_VIEW / 8 - (DistanceCounter.FIELD_OF_VIEW / 2) - (DistanceCounter.FIELD_OF_VIEW / 16);
                Vector3 direction = Quaternion.AngleAxis(sightAngle, new Vector3(0, 1, 0)) * this.transform.forward;
                Vector3 endPoint = currPosition + direction * distances[i - 1];
                endPoints.Add(endPoint);
                //setAnObstacle(endPoint);
            }
        }

        //Debug.Log("distance:" + string.Join(" ", distances));
        //Debug.Log("before:" + string.Join(" ", endPoints));
        endPoints = smoothObstacles(endPoints);
        //Debug.Log("after:" + string.Join(" ", endPoints));
        foreach(Vector3 endPoint in endPoints)
        {
            setAnObstacle(endPoint);
        }
    }

    public void resetMap()
    {
        map = new GridMap();
    }

    public bool ifItIsPossibleThisWayPassable(float oriX, float oriY, float endX, float endY)
    {
        return map.ifMeetAnObstacleAlongThisWay(oriX, oriY, endX, endY);
    }

    public bool ifThisLineIsChecked(Vector3 beforePosition, Vector3 afterPosition)
    {
        bool b = map.ifThisLineIsChecked(beforePosition.x, beforePosition.z, afterPosition.x, afterPosition.z);
        b = b & map.ifThisLineIsChecked(beforePosition.x + 4, beforePosition.z, afterPosition.x - 4, afterPosition.z);
        b = b & map.ifThisLineIsChecked(beforePosition.x, beforePosition.z + 4, afterPosition.x, afterPosition.z - 4);
        return b;
    }

    public bool ifThisPointIsChecked(Vector3 position)
    {
        return map.ifThisPointIsChecked(position.x, position.z);
    }
}
