using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSimulator : MonoBehaviour
{
    public float wantedAngle; // wanted angle Y
    public InterfaceManager uiManager;
    private float targetAngle;
    private float rotatedVolocity;
    // Start is called before the first frame update
    void Start()
    {
        wantedAngle = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(uiManager.getGameMode() == InterfaceManager.SELF_DRIVING_MODE)
        {
            return;
        }
        float currAngle = transform.eulerAngles.y;
        if (Mathf.Abs(wantedAngle - currAngle) < 1.5f)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, wantedAngle, transform.eulerAngles.z);
        }
        else
        {
            targetAngle = Mathf.SmoothDamp(currAngle, wantedAngle, ref rotatedVolocity, 0.4f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
        }
    }

    public void setRotatedAngle(float theAngle)
    {
        if (theAngle > 360 || theAngle < -360)
        {
            this.wantedAngle = theAngle % 360;
        }
        else
        {
            this.wantedAngle = theAngle;
        }
    }

    public float getRotatedAngle()
    {
        return this.wantedAngle;
    }
}