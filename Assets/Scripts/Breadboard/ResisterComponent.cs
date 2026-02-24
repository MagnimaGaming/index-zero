using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResisterComponent : CircuitComponent
{
    [Header("Resistance")]
    public float resistance = 220f;

    public override void Simulate(CircuitSolver solver)
    {
        //solver read this.resistance no to show anything
    }

    public override void ResetState()
    {
        //no reset required
    }
}
