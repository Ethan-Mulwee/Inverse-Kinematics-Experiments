using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IK;
using UnityEditor;
using System;
using System.Threading;
using System.Diagnostics;

[ExecuteAlways]
public class IKComponent : MonoBehaviour
{
    [SerializeField] uint Iterations;
    [SerializeField] uint SegmentCount;
    [SerializeField] float SegmentLength;
    Vector3 TargetPos = Vector3.zero;
    Vector3 LerpPos = Vector3.zero; 
    Vector3 RealPos = Vector3.zero;
    Vector3 VisualPos = Vector3.zero;
    Limb limb;
    float LastStepDist;
    void Update() {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 2f, Vector3.down, out hit)) {
            TargetPos = hit.point;
            if (Vector3.Distance(transform.position, RealPos) > SegmentCount*SegmentLength-0.05f) {
                LerpPos = TargetPos;
                LastStepDist = Vector3.Distance(RealPos, LerpPos);
            }
        }
        float StepDist = Vector3.Distance(RealPos,LerpPos);
        RealPos = Vector3.Lerp(RealPos, LerpPos, 0.05f);
        StepDist = Vector3.Distance(RealPos,LerpPos);
        float StepHeight = Mathf.Clamp(-1f*((StepDist-(LastStepDist))*StepDist), 0, float.PositiveInfinity);
        VisualPos = RealPos + new Vector3(0, StepHeight, 0);
        limb.FABRIK(Iterations, gameObject.transform.position, VisualPos);
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
        Gizmos.DrawSphere(LerpPos, 0.2f);
        Gizmos.DrawSphere(VisualPos, 0.1f);
    }
}
