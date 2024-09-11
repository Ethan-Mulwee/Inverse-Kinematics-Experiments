using UnityEngine;
using IK;
using UnityEditor;
using System;

[ExecuteAlways] [Serializable]
public class IKLimb : MonoBehaviour
{
    public float animTime = 0.1f;
    public float leanMultipler = 35f;
    [SerializeField] uint Iterations;
    public float segmentLength;
    [SerializeField] GameObject poleTarget;
    [SerializeField] GameObject rayTarget;
    Vector3 Target = Vector3.zero;
    Vector3 animTarget1 = Vector3.zero;
    public Vector3 animTarget2 = Vector3.zero;
    public Vector3 End = Vector3.zero;
    Limb limb;
    Vector3 stepVelocity = Vector3.zero;
    public bool Grounded;


    void OnEnable() {
        Refresh();
    }
    void OnValidate() {
        Refresh();
    }
    public void Refresh() {
        limb = new Limb(2,segmentLength);
    }

    void Update()
    {
        GetVelocity();
        GetTarget();
        CheckOverextension();
        animTarget2 = Vector3.SmoothDamp(animTarget2, animTarget1, ref stepVelocity, animTime);
        float stepDistance = Vector3.Distance(animTarget2, animTarget1);
        float lastDistance = Vector3.Distance(lastTarget, animTarget1);
        float stepHeight = Mathf.Clamp(-0.5f*((stepDistance-(lastDistance))*stepDistance), 0, 1);
        End = animTarget2 + stepHeight*transform.up;
        End = Vector3.ClampMagnitude(End-transform.position, segmentLength*2)+transform.position;
        limb.PoleTarget(poleTarget.transform.position);
        limb.FABRIK(Iterations, gameObject.transform.position, End);
    }

    private void CheckOverextension()
    {
        if (Vector3.Distance(transform.position, End) > 2 * segmentLength) {
            lastTarget = animTarget1;
            animTarget1 = Target;
            Grounded = false;
        } else {
            Grounded = true;
        }
    }

    private void GetTarget()
    {
        RaycastHit hit;
         Vector3 direction = Vector3.Normalize(rayTarget.transform.position + (smoothVelocity * leanMultipler) - transform.position);
        //Vector3 direction = Vector3.Normalize(rayTarget.transform.position - transform.position);
        //Debug.DrawRay(transform.position, direction);
        // int count = 0;
        // int i = 10;
        // while (count < i) {
        //     count++;
        //     Debug.DrawRay(transform.position+(direction*((float)count/i)), Vector3.Lerp(direction, transform.up*-1, (float)count/i));
        //     if (Physics.SphereCast(transform.position+(direction*((float)count/i)), 0.5f, Vector3.Lerp(direction, Vector3.down, (float)count/i), out hit, segmentLength*2)) {
        //         Target = hit.point;
        //         break;
        //     }
        // }
        if (Physics.SphereCast(transform.position, 0.5f, direction, out hit))
        {
            Target = hit.point;
        }
    }

    Vector3 perviousPosition = Vector3.zero;
    Vector3 bodyVelocity = Vector3.zero;
    Vector3 smoothVelocity = Vector3.zero;
    Vector3 smoothDampVelocity = Vector3.zero;
    void GetVelocity() {
        bodyVelocity = transform.position - perviousPosition;
        perviousPosition = transform.position;
        smoothVelocity = Vector3.SmoothDamp(smoothVelocity, bodyVelocity, ref smoothDampVelocity, 0.1f);
    }

    Vector3 lastTarget;
    public void Step() {
        //min step dist
        if (Vector3.Distance(Target, End) > 0.7f) {
            lastTarget = animTarget1;
            animTarget1 = Target;
        }
    }

    void OnDrawGizmos() {

        foreach(Segment segment in limb.Segments) {
            Handles.color = Color.black;
            Handles.DrawLine(segment.a, segment.b, 6f);
        }
        //Gizmos.DrawSphere(lastTarget,0.3f);
        Gizmos.DrawSphere(End, 0.1f);
        Gizmos.DrawSphere(Target, 0.3f);
    }

}
