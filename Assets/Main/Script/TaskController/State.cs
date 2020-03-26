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
}
