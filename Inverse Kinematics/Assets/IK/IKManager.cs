
using UnityEngine;
using IK;
using System.Collections.Generic;
using System;

[ExecuteAlways]
public class IKManager : MonoBehaviour
{
    float Timer = 0;
    [SerializeField] List<LimbPairing> limbs = new List<LimbPairing>();

    void Update() {
        Timer += Time.deltaTime;
        if (Timer > 0.4f) {
            Timer = 0;
            foreach(LimbPairing limbPair in limbs) {
                limbPair.Step();
            }
        }
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
}
