using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowDrone : MonoBehaviour
{
    private Transform dronePosition;
    private Transform thisPosition;
    // Start is called before the first frame update
    void Start()
    {
        dronePosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        thisPosition = this.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        thisPosition.position = dronePosition.position;
        Vector3 eulerAngles = dronePosition.eulerAngles;
        eulerAngles.x = thisPosition.eulerAngles.x;
        eulerAngles.z = thisPosition.eulerAngles.z;

        thisPosition.eulerAngles = eulerAngles; // only follow 'y'
    }
}
