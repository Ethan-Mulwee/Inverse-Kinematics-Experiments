
using UnityEngine;
using IK;
using System.Collections.Generic;
using System;

[ExecuteAlways]
public class IKManager : MonoBehaviour
{
    [Range(0f,1f)]
    [SerializeField] float stepTime = 0.4f;
    [Range(0f, 1f)]
    [SerializeField] float animTime = 0.1f;
    [Range(0f, 60f)]
    [SerializeField] float leanMultipler = 35f;
    [SerializeField] float segmentLength = 1f;
    [SerializeField] List<LimbPairing> limbs = new List<LimbPairing>();
    float Timer = 0;

    void updateProperties() {
        foreach (LimbPairing limbPair in limbs) {
            limbPair.leadingLimb.animTime = animTime;
            limbPair.trailingLimb.animTime = animTime;
            limbPair.leadingLimb.leanMultipler = leanMultipler;
            limbPair.trailingLimb.leanMultipler = leanMultipler;
            limbPair.leadingLimb.segmentLength = segmentLength;
            limbPair.trailingLimb.segmentLength = segmentLength;
            limbPair.leadingLimb.Refresh();
            limbPair.trailingLimb.Refresh();
        }
    }

    void OnEnable() {
        updateProperties();
    }
    void OnValidate() {
        updateProperties();
    }

    void Update() {
        Timer += Time.deltaTime;
        if (Timer > stepTime) {
            Timer = 0;
            foreach(LimbPairing limbPair in limbs) {
                limbPair.Step();
            }
        }
        GetOrientation();
    }
    [Serializable]
    class LimbPairing {
        [SerializeField] public IKLimb leadingLimb;
        [SerializeField] public IKLimb trailingLimb;
        bool LimbSelector = true;
        public void Step() {
            if (LimbSelector) leadingLimb.Step();
            else trailingLimb.Step();
            LimbSelector = !LimbSelector;
        }
    }
    public Vector3 AveragePosition() {
        Vector3 total = Vector3.zero;
        int count = 0;
        foreach (LimbPairing limbPair in limbs) {
            total += limbPair.leadingLimb.End;
            total += limbPair.trailingLimb.End;
            count += 2;
        }
        return total/count;
    }
    public float GroundedFactor() {
        int total = 0;
        int count = 0;
        foreach (LimbPairing limbPair in limbs) {
            if (limbPair.leadingLimb.Grounded) total += 1;
            if (limbPair.trailingLimb.Grounded) total += 1;
            count += 2;
        }
        return (float)total/count;
    }
    public Vector3 GetOrientation() {
        Vector3 p0 = limbs[0].leadingLimb.animTarget2;
        Vector3 p1 = limbs[0].trailingLimb.animTarget2;
        Vector3 p2 = limbs[1].leadingLimb.animTarget2;
	    Vector3 vec1 = p0-p1;
	    Vector3 vec2 = p2-p1;
	    Vector3 cross1 = Vector3.Cross(vec1, vec2);
	    //Debug.DrawRay(transform.position, cross1);
	    Vector3 g0 = limbs[1].trailingLimb.animTarget2;
	    Vector3 g1 = limbs[2].leadingLimb.animTarget2;
	    Vector3 g2 = limbs[2].trailingLimb.animTarget2;
	    Vector3 vec3 = g1-g0;
	    Vector3 vec4 = g2-g1;
	    Vector3 cross2 = Vector3.Cross(vec3, vec4);
        Debug.DrawRay(transform.position, (cross1.normalized+cross2.normalized)*0.5f);
	    return (cross1.normalized+cross2.normalized)*0.5f; 

    }
}
