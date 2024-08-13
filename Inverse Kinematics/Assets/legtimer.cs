using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class legtimer : MonoBehaviour
{
[SerializeField] IKComponent leg1;
[SerializeField] IKComponent leg2;
float Timer;
bool LegSelection;

void Update() {
    Timer += Time.deltaTime;
    if (Timer > 0.5f) {
        if (LegSelection) leg1.Selected = true;
        else leg2.Selected = true;
        LegSelection = !LegSelection;
        Timer = 0f;
    }
}
}
