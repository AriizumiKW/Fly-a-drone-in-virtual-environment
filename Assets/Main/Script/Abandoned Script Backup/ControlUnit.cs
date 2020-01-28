using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUnit : MonoBehaviour
{
    // Start is called before the first frame update
    
    private Rigidbody rg;
    private Transform tf;

    private Vector3 forward;
    private Vector3 back;
    private Vector3 left;
    private Vector3 right;
    private Vector3 _up;

    private void Start()
    {
        rg = this.GetComponent<Rigidbody>();
        tf = this.GetComponent<Transform>();
        forward = GameObject.FindGameObjectWithTag("Front").GetComponent<Transform>().localPosition;
        forward = forward / forward.magnitude;
        left = GameObject.FindGameObjectWithTag("Left").GetComponent<Transform>().localPosition;
        left = left / left.magnitude;
        back = GameObject.FindGameObjectWithTag("Back").GetComponent<Transform>().localPosition;
        back = back / back.magnitude;
        right = GameObject.FindGameObjectWithTag("Right").GetComponent<Transform>().localPosition;
        right = right / right.magnitude;
        _up = Vector3.Cross(forward, right);
    }

    private void FixedUpdate()
    {
        // quadFront = _front.position;

        float accelaration = 15.0f;

        if (Input.GetKey(KeyCode.Space))
        {
            rg.AddRelativeForce(_up * accelaration * rg.mass);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rg.AddRelativeForce(forward * accelaration * rg.mass);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rg.AddRelativeForce(left * accelaration * rg.mass);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rg.AddRelativeForce(back * accelaration * rg.mass);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rg.AddRelativeForce(right * accelaration * rg.mass);
        }
    }
    
    /*
    private KeyboardInput _currInput;
    private GameObject _currDrone;
    private Rigidbody _currDronePhysicalSystem;
    private Transform _currTransform;
    private Transform _front;
    private Transform _back;
    private Transform _left;
    private Transform _right;

    private float gravityAcceleration = 10.0f;

    enum KeyboardInput
    {
        W,
        A,
        S,
        D,
        SPACE,
        ESC,
        NO_INPUT
    }
    void Start()
    {
        _currInput = KeyboardInput.NO_INPUT;
        _currDrone = GameObject.FindGameObjectWithTag("currDrone");
        _currDronePhysicalSystem = _currDrone.GetComponent<Rigidbody>();
        _currTransform = _currDrone.GetComponent<Transform>();
        _front = GameObject.FindGameObjectWithTag("Front").GetComponent<Transform>();
        _back = GameObject.FindGameObjectWithTag("Back").GetComponent<Transform>();
        _left = GameObject.FindGameObjectWithTag("Left").GetComponent<Transform>();
        _right = GameObject.FindGameObjectWithTag("Right").GetComponent<Transform>();
    }

    // Update is called once per framef
    void FixedUpdate()
    {
        _currDronePhysicalSystem.centerOfMass = _currTransform.position;
        //_currDronePhysicalSystem.AddForce(_currDronePhysicalSystem.mass * Vector3.down * gravityAcceleration);
        Vector3 quadForward = _front.position;
        Vector3 quadBack = _back.position;
        Vector3 quadLeft = _left.position;
        Vector3 quadRight = _right.position;

        Vector3 directionUp = Vector3.Cross(quadForward, quadLeft);
        directionUp = directionUp / directionUp.magnitude;

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log(directionUp);
            //Debug.Log(quadForward);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadBack);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadLeft);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadRight);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadForward);
        }
        if (Input.GetKey(KeyCode.W))
        {
            //_currDronePhysicalSystem.AddForce(_currDronePhysicalSystem.mass * Vector3.forward * 10.0f);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadBack);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadLeft);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadRight);
        }
        if (Input.GetKey(KeyCode.A))
        {
            //_currDronePhysicalSystem.AddForce(_currDronePhysicalSystem.mass * Vector3.left * 10.0f);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadBack);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadRight);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadForward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            //_currDronePhysicalSystem.AddForce(_currDronePhysicalSystem.mass * Vector3.back * 10.0f);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadLeft);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadRight);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadForward);
        }
        if (Input.GetKey(KeyCode.D))
        {
            //_currDronePhysicalSystem.AddForce(_currDronePhysicalSystem.mass * Vector3.right * 10.0f);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadBack);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadLeft);
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * 5.0f, quadForward);

        }
        // 测试代码
        if (Input.GetKey(KeyCode.R))
        {
            float accelaration = 15.0f;
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * accelaration, _currTransform.position);
        }
        if (Input.GetKey(KeyCode.T))
        {
            float accelaration = 9.8f;
            _currDronePhysicalSystem.AddForceAtPosition(_currDronePhysicalSystem.mass * directionUp * accelaration, _currTransform.position);
        }
    }
    */
}
