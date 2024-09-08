using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(IKManager), typeof(Rigidbody))]
public class IKBodyController : MonoBehaviour
{
    Rigidbody rb;
    IKManager ik;
    Vector3 input;
    Vector3 target;
    [SerializeField] float Height =  1.5f;

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


    void Update() {
        GetInput();
        GetTarget();
        rb.AddForce(input*8*Time.deltaTime, ForceMode.Impulse);
        Vector3 targetVector = target-transform.position;
        targetVector = new Vector3(targetVector.x, Mathf.Clamp(targetVector.y, -1f, float.PositiveInfinity), targetVector.z);
        Debug.DrawRay(transform.position, targetVector);
        rb.AddForce(Physics.gravity*-1*Time.deltaTime, ForceMode.Impulse);
        rb.AddForce(targetVector*Time.deltaTime*10, ForceMode.Impulse);
    }

    void GetInput() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical"));
    }

    void GetTarget() {
        Vector3 averagePos = ik.AveragePosition();
        target = new Vector3(transform.position.x, averagePos.y+Height, transform.position.z);
    }
    void OnDrawGizmos() {
        Gizmos.DrawSphere(target, 0.5f);
    }
}
