
using UnityEngine;
using IK;
using System;

namespace IK {
    [Serializable]
    public class Limb
    {
        public Segment[] Segments;

        public Limb(uint segCount, float segLength) {
            Segments = new Segment[segCount];
            for (int i = 0; i < Segments.Length; i++) {
                Segments[i] = new Segment(segLength);
            }
        }

        void Backwards(Vector3 Target) {
            Segment Last = Segments[Segments.Length-1];
            Last.b = Target;
            Last.calculateA(Target);
            for (int i = Segments.Length-2; i > -1; i--) {
                Segment segment = Segments[i];
                segment.b = Segments[i+1].a;
                segment.calculateA(segment.b);
            }
        }

        void Fowards(Vector3 Target) {
            Segment First = Segments[0];
            First.a = Target;
            First.calculateB(Target);
            for (int i = 1; i < Segments.Length; i++) {
                Segment segment = Segments[i];
                segment.a = Segments[i-1].b;
                segment.calculateB(segment.a);
            }
        }
        
        public void FABRIK(uint Iterations, Vector3 Start, Vector3 Target) {
            for (int i = 0; i < Iterations; i++) {
                Backwards(Target);
                Fowards(Start);
            }
        }

        public void PoleTarget(Vector3 poleTarget)
        {
            Vector3 Start = Segments[0].a;
            Vector3 End = Segments[Segments.Length-1].b;
            Vector3 rotatePos = Vector3.Lerp(Start, End, 0.5f);
            Vector3 poleVector = poleTarget - rotatePos;
            Vector3 rotateUpDir = Start - rotatePos;
            Vector3 projectedVector = Vector3.ProjectOnPlane(poleVector, rotateUpDir);
            Vector3 differenceVector = Segments[1].a - rotatePos;
            projectedVector.Normalize();
            projectedVector = projectedVector * differenceVector.magnitude;
            Segments[1].a = projectedVector+rotatePos;
            Segments[0].b = projectedVector+rotatePos;
        }
    }
}
