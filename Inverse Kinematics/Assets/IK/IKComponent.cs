using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IK;
using Unity.VisualScripting;
using UnityEditor;

public class IKComponent : MonoBehaviour
{
    public GameObject target;
    [SerializeField] uint Iterations;
    [SerializeField] uint SegmentCount;
    [SerializeField] float SegmentLength;
    Vector3 Target = Vector3.zero;
    Limb limb;
    // void Update() {
    //     RaycastHit hit;
    //     if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
    //         Target = hit.point;
    //     }
    // }
    void OnValidate()
    {
        limb = new Limb(SegmentCount, SegmentLength);
    }
    void OnDrawGizmos() {
        limb.FABRIK(Iterations, gameObject.transform.position, target.transform.position);
        foreach(Segment segment in limb.Segments) {
            Handles.DrawLine(segment.a, segment.b, 6f);
        }
    }
}
