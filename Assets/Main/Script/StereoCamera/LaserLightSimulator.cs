using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLightSimulator : MonoBehaviour
{
    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray r = new Ray(this.transform.position, transform.forward);
        RaycastHit hitInfo;
        if(Physics.Raycast(r, out hitInfo))
        {
            distance = (r.origin - hitInfo.point).magnitude;
            this.gameObject.GetComponent<Light>().range = distance + 20.0f;

            Debug.Log(distance+"!!!!!!!!!!!");
            //Debug.DrawLine(r.origin, hitInfo.point);
        }
    }
}
