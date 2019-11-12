using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform dronePosition;
    private Transform cameraPosition;

    void Start()
    {
        dronePosition = GameObject.FindGameObjectWithTag("currDrone").GetComponent<Transform>();
        cameraPosition = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cameraPosition.position = dronePosition.position + new Vector3(8.0f, 11.0f, -9.0f);
    }
}
