using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public const float EFFECTIVE_DISTANCE = 65.0f;
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

    public void updateMap(float[] distances, float currOrientation, Vector3 currPosition)
    {
        for (int i = 1; i <= 7; i++)
        {
            float distance1 = distances[i - 1];
            float distance2 = distances[i];
            /*
            if(distance1 == DistanceCounter.INVALID_DISTANCE || distance2 == DistanceCounter.INVALID_DISTANCE)
            {
                return;
            }
            */
            distance1 -= 8.0f;
            distance2 -= 8.0f; // increase robust
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
                demoGraph.drawPoint((int)endPoint.x, (int)endPoint.z, OpenCvSharp.Scalar.Yellow, 3);
            }
        }

        List<Vector3> inliers = map.setObstacles(endPoints); // outliers filter
    }

    public void resetMap()
    {
        map = new GridMap();
    }

    public bool ifMeetAnObstacleAlongThisWay(float oriX, float oriY, float endX, float endY)
    {
        return map.ifMeetAnObstacleAlongThisWay(oriX, oriY, endX, endY);
        /*
        bool b = map.ifMeetAnObstacleAlongThisWay(oriX, oriY, endX, endY);
        b = b || map.ifMeetAnObstacleAlongThisWay(oriX + 2, oriY, endX + 2, endY);
        b = b || map.ifMeetAnObstacleAlongThisWay(oriX, oriY + 2, endX, endY + 2);
        return b;
        */
    }

    public bool ifThisLineIsChecked(Vector3 beforePosition, Vector3 afterPosition)
    {
        bool b = map.ifThisLineIsChecked(beforePosition.x, beforePosition.z, afterPosition.x, afterPosition.z);
        b = b && map.ifThisLineIsChecked(beforePosition.x + 4, beforePosition.z, afterPosition.x + 4, afterPosition.z);
        b = b && map.ifThisLineIsChecked(beforePosition.x, beforePosition.z + 4, afterPosition.x, afterPosition.z + 4);
        b = b && map.ifThisLineIsChecked(beforePosition.x - 4, beforePosition.z, afterPosition.x - 4, afterPosition.z);
        b = b && map.ifThisLineIsChecked(beforePosition.x, beforePosition.z - 4, afterPosition.x, afterPosition.z - 4);
        return b;
    }

    public bool ifThisPointIsChecked(Vector3 position)
    {
        return map.ifThisPointIsChecked(position.x, position.z);
    }
}
