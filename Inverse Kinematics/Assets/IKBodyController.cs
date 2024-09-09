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
        //rb.position = targetVector;
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, 1, 0));
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -1, 0));
        }
    }

    Vector3 velocity;
    private void bodyPosition()
    {
        Vector3 force = target - transform.position;
        force = new Vector3(force.x, Mathf.Clamp(force.y, -1f, float.PositiveInfinity), force.z);
        if(force.magnitude < 0.3f) force = force.normalized * Mathf.Sqrt(force.magnitude);
        rb.AddForce(force * Time.deltaTime * 200f);
        orientation = Vector3.SmoothDamp(orientation, ik.GetOrientation(), ref velocity, 1f);
        //transform.up = orientation;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, orientation) * transform.rotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
    }

    private void Move()
    {
        Vector3 rotatedInput = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * input;
        rb.AddForce(rotatedInput * 10 * Time.deltaTime, ForceMode.Impulse);
    }

    private void HandleGravity()
    {
        rb.AddForce(Physics.gravity * -1 * Time.deltaTime * ik.GroundedFactor(), ForceMode.Impulse);
    }

    void GetInput() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
    }

    void GetTarget() {
        Vector3 averagePos = ik.AveragePosition();
        target = new Vector3(transform.position.x, averagePos.y+Height, transform.position.z);
    }
}
