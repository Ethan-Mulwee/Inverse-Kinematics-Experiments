using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(IKManager), typeof(Rigidbody))]
public class IKBodyController : MonoBehaviour
{
    Rigidbody rb;

    void OnEnable() {
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        rb.AddForce(transform.forward*5, ForceMode.Impulse);
    }
}
