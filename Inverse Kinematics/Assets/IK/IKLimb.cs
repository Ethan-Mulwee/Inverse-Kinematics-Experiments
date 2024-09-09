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
        Debug.DrawRay(transform.position, direction);
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
        lastTarget = animTarget1;
        animTarget1 = Target;
    }

    void OnDrawGizmos() {

        foreach(Segment segment in limb.Segments) {
            Handles.color = Color.black;
            Handles.DrawLine(segment.a, segment.b, 6f);
        }
        //Gizmos.DrawSphere(lastTarget,0.3f);
        Gizmos.DrawSphere(End, 0.1f);
        //Gizmos.DrawSphere(animTarget2, 0.3f);
    }

}
