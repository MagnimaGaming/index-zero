using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperWireComponent : CircuitComponent
{
    public override void Simulate(CircuitSolver solver)
    {
        //no simulation needed for wire
    }

    public override void ResetState()
    {
        //no reset needed
    }
}
