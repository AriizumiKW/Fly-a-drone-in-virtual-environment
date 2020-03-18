using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    // store a map in memory
    private RRTDrawer demoGraph;
    private List<MapObstacle> obstacles;

    void Start()
    {
        obstacles = new List<MapObstacle>();
        demoGraph = this.gameObject.GetComponent<RRTDrawer>();
    }

    public void setAnObstacle(float x, float y)
    {
        obstacles.Add(new MapObstacle(x, y));
        demoGraph.drawObstacle((int)x, (int)y);
    }

    public void setAnObstacle(Vector3 position)
    {
        float x = position.x;
        float y = position.z;
        obstacles.Add(new MapObstacle(x, y));
        demoGraph.drawObstacle((int)x, (int)y);
    }

    public void resetMap()
    {
        obstacles.Clear();
    }

    public bool ifMeetObstacleAlongTheDirection(float oriX, float oriY, float endX, float endY)
    {
        Vector3 origin = new Vector3(oriX, oriY, 1);
        Vector3 end = new Vector3(endX, endY, 1);
        foreach(MapObstacle obstacle in obstacles)
        {
            Vector3 point = new Vector3(obstacle.getX(), obstacle.getY(), 1);
            float fProj = Vector3.Dot(point - origin, (origin - end).normalized);
            float distance = Mathf.Sqrt((point - origin).sqrMagnitude - fProj * fProj);
            if(distance <= MapObstacle.radius)
            {
                return true;
            }
        }
        return false;
    }

    public bool ifMeetObstacleAlongTheDirection(Vector3 _origin, Vector3 _end)
    {
        Vector3 origin = new Vector3(_origin.x, _origin.z, 1);
        Vector3 end = new Vector3(_end.x, _end.z, 1);
        foreach (MapObstacle obstacle in obstacles)
        {
            Vector3 point = new Vector3(obstacle.getX(), obstacle.getY(), 1);
            float fProj = Vector3.Dot(point - origin, (origin - end).normalized);
            float distance = Mathf.Sqrt((point - origin).sqrMagnitude - fProj * fProj);
            if (distance <= MapObstacle.radius)
            {
                return true;
            }
        }
        return false;
    }
}
