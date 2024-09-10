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
        var mouseInput = Input.mousePositionDelta.x;
        Cursor.lockState = CursorLockMode.Locked;
        transform.Rotate(new Vector3(0, mouseInput*0.2f, 0));
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
        if(force.magnitude < 0.5f) force = force.normalized * Mathf.Sqrt(force.magnitude);
        rb.AddForce(force * Time.deltaTime*200);
        //rb.transform.position = target;
        //rb.MovePosition(target);
        orientation = Vector3.SmoothDamp(orientation, ik.GetOrientation(), ref velocity, 1f);
        //transform.up = orientation;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, orientation) * transform.rotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
    }

    private void Move()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, transform.forward);
        Vector3 rotatedInput = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * input;
        rotatedInput = targetRotation * input;
        rb.AddForce(rotatedInput * 10 * Time.deltaTime, ForceMode.Impulse);
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
        //float basetarget = averagePos.y+Height;
        Vector3 basetarget = new Vector3(transform.position.x,averagePos.y, transform.position.z);
        target = averagePos + transform.up.normalized * Height;
    }
    void OnDrawGizmos() {
        Gizmos.DrawSphere(target, 0.5f);
    }
}
