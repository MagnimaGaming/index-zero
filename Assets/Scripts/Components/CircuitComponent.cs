using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CircuitComponent : MonoBehaviour
{
    public ComponentLeg[] legs;

    public abstract void Simulate(CircuitSolver solver);
    public abstract void ResetState();

    public bool IsFullyConnected()
    {
        foreach(ComponentLeg leg in legs)
        {
            if(leg.node == null) return false;
        }
        return true;
    }

    protected float GetVoltageDrop(int legA, int legB)
    {
        if (legs[legA].node == null || legs[legB].node == null) return 0f;

        return legs[legA].node.voltage - legs[legB].node.voltage;
    }
}
