using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(IKManager), typeof(Rigidbody))]
public class IKBodyController : MonoBehaviour
{
    Rigidbody rb;
    IKManager ik;
    Vector3 input;
    Vector3 target;
    Vector3 orientation = Vector3.up;
    [SerializeField] float Height =  2f;

    [Header("PID Settings")]
    [SerializeField] float kp = 100f;
    [SerializeField] float ki = 0.01f;
    [SerializeField] float kd = 10f;
    Vector3 cumulativeError = Vector3.zero;
    Vector3 perviousError = Vector3.zero;
    float jumpCharge;

    void OnValidate() {
        Intialize();
    }
    void OnEnable() {
        Intialize();
    }
    void Intialize() {
        rb = GetComponent<Rigidbody>();
        ik = GetComponent<IKManager>();
    }


    void Update()
    {
        GetInput();
        GetTarget();
        Move();
        HandleGravity();
        bodyPosition();
        var mouseInput = Input.mousePositionDelta.x;
        //Cursor.lockState = CursorLockMode.Locked;
        //transform.Rotate(new Vector3(0, mouseInput*0.2f, 0));
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, 1, 0));
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -1, 0));
        }
        if (Input.GetKey(KeyCode.Space)) {
            jumpCharge += 1*Time.deltaTime;
            jumpCharge = Mathf.Clamp(jumpCharge, 0, 1);
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            rb.AddForce(transform.up*jumpCharge*600);
            jumpCharge = 0;
        }
        transform.Rotate(new Vector3(0, Input.GetAxis("Camera")*ik.GroundedFactor(), 0));
    }

    Vector3 velocity;
    private void bodyPosition()
    {
        Vector3 force = target - transform.position;
        force = new Vector3(force.x, Mathf.Clamp(force.y, -1f, float.PositiveInfinity), force.z);
        if(force.magnitude < 0.5f) force = force.normalized * Mathf.Sqrt(force.magnitude);
        rb.AddForce(Vector3.Project(PID(transform.position, target)*ik.GroundedFactor(), transform.up));
        orientation = Vector3.SmoothDamp(orientation, ik.GetOrientation(), ref velocity, 0.5f);
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, orientation) * transform.rotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
    }

    Vector3 PID(Vector3 position, Vector3 target) {
        Vector3 force = Vector3.zero;
        Vector3 error = target-position;
        Vector3 deltaError = error-perviousError;
        cumulativeError += error;
        force += error*kp;
        force += cumulativeError*ki;
        force += deltaError*kd;
        
        perviousError = error;

        return force;
    }

    private void Move()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, transform.forward);
        Quaternion targetRotation2 = Quaternion.FromToRotation(Vector3.up, transform.up);
        Vector3 rotatedInput = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * input;
        rotatedInput = transform.rotation * input;
        rb.AddForce(rotatedInput * 6 * Time.deltaTime*ik.GroundedFactor(), ForceMode.Impulse);
    }

    private void HandleGravity()
    {
        rb.AddForce(Physics.gravity * -1 * Time.deltaTime * ik.GroundedFactor(), ForceMode.Impulse);
    }

    void GetInput() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
    }

    float velocity2;
    //Fix to be averagePos + height along orientation
    void GetTarget() {
        Vector3 averagePos = ik.AveragePosition();
        target = Vector3.Project(averagePos, transform.up.normalized) + Vector3.ProjectOnPlane(transform.position, transform.up) + transform.up.normalized * Height;
    }

    void OnDrawGizmos() {
        //Gizmos.DrawSphere(ik.AveragePosition(), 1f);
        //Gizmos.DrawSphere(target, 0.7f);
    }
}
