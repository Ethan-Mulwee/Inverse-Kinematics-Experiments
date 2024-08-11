using UnityEngine;

namespace IK {
    public class Segment
    {
        public float Length;
        public Vector3 a;
        public Vector3 b;

        public Segment(float Length_) {
            Length = Length_;
            a = new Vector3();
            b = new Vector3();
        }

        public void calculateB(Vector3 Target) {
            Vector3 dir = Target-b;
            dir.Normalize();
            Vector3 vec = dir*-Length;
            b = vec + a;
        }
        public void calculateA(Vector3 Target) {
            Vector3 dir = Target-a;
            dir.Normalize();
            Vector3 vec = dir*-Length;
            a = vec + b;
        }
    }
}
