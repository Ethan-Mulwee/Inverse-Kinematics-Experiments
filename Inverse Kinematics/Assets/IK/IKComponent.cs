using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IK;
using UnityEditor;
using System;
using System.Threading;

[ExecuteAlways]
public class IKComponent : MonoBehaviour
{
    [SerializeField] uint Iterations;
    [SerializeField] uint SegmentCount;
    [SerializeField] float SegmentLength;
    [SerializeField] GameObject poleTarget;
    Vector3 TargetPos = Vector3.zero;
    Vector3 LerpPos = Vector3.zero; 
    Vector3 RealPos = Vector3.zero;
    Vector3 VisualPos = Vector3.zero;
    Limb limb;
    Limb limb2;
    Vector3 Velocity = Vector3.zero;
    float LastStepDist;
    public bool Selected = false;
    void Update()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 2f, Vector3.down, out hit))
        {
            TargetPos = hit.point;
            if (Selected || (Vector3.Distance(transform.position, RealPos) > SegmentCount*SegmentLength)) {
                Selected = false;
                LerpPos = TargetPos;
                LastStepDist = Vector3.Distance(RealPos, LerpPos);
                Velocity = Vector3.zero;
            }
        }
        float StepDist = Vector3.Distance(RealPos,LerpPos);
        //RealPos = Vector3.Lerp(RealPos, LerpPos, 0.035f);
        RealPos = Vector3.SmoothDamp(RealPos, LerpPos, ref Velocity, 0.04f);
        StepDist = Vector3.Distance(RealPos,LerpPos);
        float StepHeight = Mathf.Clamp(-0.1f*((StepDist-(LastStepDist))*StepDist), 0, float.PositiveInfinity);
        VisualPos = RealPos + new Vector3(0, StepHeight, 0);
        Pole(limb);
        limb.FABRIK(Iterations, gameObject.transform.position, VisualPos);
        //Pole(limb2);
    }

    private void Pole(Limb limb)
    {

        Vector3 rotatePos = Vector3.Lerp(transform.position, VisualPos, 0.5f);
        Vector3 poleVector = poleTarget.transform.position - rotatePos;
        Vector3 rotateUpDir = transform.position - rotatePos;
        Debug.DrawRay(rotatePos, poleVector);
        Debug.DrawRay(rotatePos, rotateUpDir);
        Vector3 projectedVector = Vector3.ProjectOnPlane(poleVector, rotateUpDir);
        //Debug.DrawRay(rotatePos, projectedVector);
        Vector3 differenceVector = limb.Segments[1].a - rotatePos;
        Debug.DrawRay(rotatePos, differenceVector);
        projectedVector.Normalize();
        projectedVector = projectedVector * differenceVector.magnitude;
        Debug.DrawRay(rotatePos, projectedVector);
        limb.Segments[1].a = projectedVector+rotatePos;
        limb.Segments[0].b = projectedVector+rotatePos;
    }

    void OnValidate()
    {
        limb = new Limb(SegmentCount, SegmentLength);
    }
    void OnDrawGizmos() {

        foreach(Segment segment in limb.Segments) {
            Handles.color = Color.black;
            Handles.DrawLine(segment.a, segment.b, 6f);
        }
        Gizmos.DrawSphere(TargetPos,0.3f);
        //Handles.DrawLine(transform.position, TargetPos, 10f);
        //Gizmos.DrawSphere(Vector3.Lerp(transform.position, TargetPos, 0.5f), 0.5f);
        // Gizmos.DrawSphere(LerpPos, 0.2f);
        Gizmos.DrawSphere(VisualPos, 0.1f);
    }
}
