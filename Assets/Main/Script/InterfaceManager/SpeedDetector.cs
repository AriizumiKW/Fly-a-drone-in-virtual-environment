using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDetector : MonoBehaviour
{
    // This gameobject: InterfaceManager

    public Rigidbody drone;
    public PowerfulEngine engine;
    private InterfaceManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = this.GetComponent<InterfaceManager>();
        speedLastFrame = new Vector3(0, 0, 0);
    }

    private Vector3 speedLastFrame;
    void FixedUpdate()
    {
        Vector3 velocity = drone.velocity;
        Vector3 acceleration = (drone.velocity - speedLastFrame) / Time.deltaTime;

        speedLastFrame = velocity;
        uiManager.updateVelocityAcceleration(velocity, acceleration);
    }
}
