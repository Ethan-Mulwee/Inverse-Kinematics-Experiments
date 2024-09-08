using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IK;
using UnityEditor;
using System;
using System.Threading;

[ExecuteAlways] [Serializable]
public class IKLimb : MonoBehaviour
{
    [SerializeField] uint Iterations;
    [SerializeField] float SegmentLength;
    [SerializeField] GameObject poleTarget;
    [SerializeField] GameObject rayTarget;
    Vector3 Target = Vector3.zero;
    Vector3 End = Vector3.zero;
    Limb limb;
    Vector3 Velocity = Vector3.zero;

    void OnEnable() {
        limb = new Limb(2, SegmentLength);
    }
    void OnValidate() {
        limb = new Limb(2, SegmentLength);
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 direction = Vector3.Normalize(rayTarget.transform.position-transform.position);
        Debug.DrawRay(transform.position, direction);
        if (Physics.SphereCast(transform.position, 0.5f, direction, out hit))
        {
            Target = hit.point;
            if (Vector3.Distance(transform.position, End) > 2*SegmentLength) {
                End = Target;
            }
        }
        limb.PoleTarget(poleTarget.transform.position);
        limb.FABRIK(Iterations, gameObject.transform.position, End);
    }

    public void Step() {
        End = Target;
    }

    private void PoleTarget(Limb limb, Vector3 End)
    {
        Vector3 rotatePos = Vector3.Lerp(transform.position, End, 0.5f);
        Vector3 poleVector = poleTarget.transform.position - rotatePos;
        Vector3 rotateUpDir = transform.position - rotatePos;
        Vector3 projectedVector = Vector3.ProjectOnPlane(poleVector, rotateUpDir);
        Vector3 differenceVector = limb.Segments[1].a - rotatePos;
        projectedVector.Normalize();
        projectedVector = projectedVector * differenceVector.magnitude;
        limb.Segments[1].a = projectedVector+rotatePos;
        limb.Segments[0].b = projectedVector+rotatePos;
    }

    void OnDrawGizmos() {

        foreach(Segment segment in limb.Segments) {
            Handles.color = Color.black;
            Handles.DrawLine(segment.a, segment.b, 6f);
        }
        Gizmos.DrawSphere(Target,0.3f);
        Gizmos.DrawSphere(End, 0.1f);
    }

}
