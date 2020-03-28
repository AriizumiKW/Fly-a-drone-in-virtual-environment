using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    private Vector3 position; // current position
    private float orientation; // current eulerAngles.y

    public State (Vector3 _position, float _orientation)
    {
        this.position = _position;
        this.orientation = _orientation;
    }

    public Vector3 getCurrentPosition()
    {
        return position;
    }

    public float getCurrentOrientation()
    {
        return orientation;
    }

    public static bool isVaild(State left, State right)
    {
        float leftCameraOrientation = left.getCurrentOrientation();
        float rightCameraOrientation = right.getCurrentOrientation();
        if(leftCameraOrientation != rightCameraOrientation)
        {
            return false;
        }
        Vector3 dronePositionWhileLeftCameraCapture = left.getCurrentPosition();
        Vector3 dronePositionWhileRightCameraCapture = right.getCurrentPosition();
        Vector3 diff = dronePositionWhileLeftCameraCapture - dronePositionWhileRightCameraCapture;
        float uncertainty = Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.z);
        if (uncertainty > 2)
        {
            return false;
        }
        return true;
    }
}
